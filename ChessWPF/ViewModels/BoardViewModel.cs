using ChessWPF.Commands;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public sealed class BoardViewModel : ViewModelBase
    {
        private Board board;
        private CellViewModel[][] cellViewModels;
        private List<Cell> backupCellsToUpdate;

        public BoardViewModel()
        {
            Board = new Board();
            cellViewModels = new CellViewModel[8][];
            MatchCellViewModelsToCells();
            Board.TurnColor = PieceColor.White;
            backupCellsToUpdate = new List<Cell>();
            EndGameCommand = new EndGameCommand(this);
        }

        public Board Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;
                OnPropertyChanged(nameof(Board));
            }
        }

        public CellViewModel[][] CellViewModels
        {
            get
            {
                return cellViewModels;
            }
            set
            {
                cellViewModels = value;
                OnPropertyChanged(nameof(CellViewModels));
            }
        }

        public string GameResult
        {
            get { return Board.GameResult; }
            set
            {
                Board.GameResult = value;
                OnPropertyChanged(nameof(GameResult));
            }
        }

        public string FenAnnotation
        {
            get => Board.FenAnnotation;
            set
            {
                OnPropertyChanged(nameof(FenAnnotation));
            }

        }

        public bool GameHasEnded
        {
            get { return Board.GameHasEnded; }
            set
            {
                Board.GameHasEnded = value;
                OnPropertyChanged(nameof(GameHasEnded));
            }
        }

        public bool GameHasStarted
        {
            get { return Board.GameHasStarted; }
            set
            {
                Board.GameHasStarted = value;
                OnPropertyChanged(nameof(GameHasStarted));
            }
        }

        public ICommand EndGameCommand { get; set; }

        public void MatchCellViewModelsToCells()
        {
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                CellViewModels[row] = new CellViewModel[8];
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    CellViewModels[row][col] = new CellViewModel(board.Cells[row, col]);
                    if (Board.Cells[row, col].Piece != null)
                    {
                        CellViewModels[row][col].UpdateCellImage();
                    }
                }
            }
        }
        public void StartGame()
        {
            Board.TurnColor = PieceColor.White;

            MarkWhichPiecesCanBeSelected();
            Board.CalculatePossibleMoves();
            MarkWhichPiecesCanBeSelected();
            UpdateCellViewModels();
            Board.UpdateFenAnnotation();
            FenAnnotation = Board.FenAnnotation;
        }

        public void ResetBoard()
        {
            if (Board.BackupCells.Count > 0)
            {
                RestoreAllBackupCells();
                UpdateCellViewModelsOfBackupCells();
            }
            Board = new Board();
            GameHasStarted = false;
            GameHasEnded = false;
            GameResult = null;
            ResetMatchCellViewModelsToCells();
            StartGame();
        }



        public void PrepareForNextTurn()
        {
            if (GameHasEnded)
            {
                return;
            }
            if (Board.BackupCells.Count > 0)
            {
                MakeAllPiecesUnselectable();
                return;
            }
            UpdateCellViewModels();
            var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);
            CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = false;
            Board.CalculatePossibleMoves();
            MakeAllPiecesUnselectable();
            if (Board.CheckForGameEnding())
            {
                GameHasEnded = true;
            }
            else
            {
                MarkWhichPiecesCanBeSelected();
                //UpdateCellViewModels();
            }
            FenAnnotation = Board.FenAnnotation;
        }


        public void MovePiece(Cell cell, Cell selectedCell)
        {
            var move = Board.MovePiece(cell, selectedCell);
            if (Board.PromotionMove != null)
            {
                MakeAllPiecesUnselectable();
                UpdateCellViewModelsForPromotion();
            }
            if (Board.BackupCells.Count == 0)
            {
                FinishMove(move, selectedCell);
            }
        }

        public void UndoMove()
        {
            var move = Board.Moves.Peek();
            if (Board.PromotionMove != null)
            {
                RestoreAllBackupCells();
                Board.UndoPromotionMove();
                MarkWhichPiecesCanBeSelected();
                UpdateCellViewModels();
                Board.CalculatePossibleMoves();
                var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);

                if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King).IsInCheck)
                {
                    CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
                }
                UpdateCellViewModelsOfBackupCells();

                Board.PromotionMove = null;
            }
            else
            {
                if (move.IsPromotionMove)
                {
                    UpdateCellViewModelsOfBackupCellsOnUndoMove(move);
                }
                Board.UndoMove();
                if (!Board.Moves.Any())
                {
                    GameHasStarted = false;
                }
                if (GameHasEnded)
                {
                    GameHasEnded = false;
                    GameResult = null;
                }
                PrepareForNextTurn();
            }
        }

        public void PromotePiece(PieceType pieceType)
        {
            Board.PromotePiece(pieceType);
            var cell = Board.PromotionMove.CellTwoAfter;
            //cell.Piece = PieceConstructor.ConstructPieceByType(pieceType, TurnColor, cell);
            CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = false;
            CellViewModels[cell.Row][cell.Col].UpdateCellImage();
            RestoreBackupCells();
            CellViewModels[Board.PromotionMove.CellOneBefore.Row][Board.PromotionMove.CellOneBefore.Col].Cell.Piece = null;
            FinishMove(Board.PromotionMove, null);
            Board.PromotionMove = null;
        }

        public void EndGameByTimeOut(PieceColor color)
        {
            GameResult = $"{color.ToString()} wins by timeout!";

            this.GameHasEnded = true;
            MakeAllPiecesUnselectable();
        }

        private void ResetMatchCellViewModelsToCells()
        {
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    cellViewModels[row][col].Cell = board.Cells[row, col];
                    cellViewModels[row][col].UpdateCellImage();
                    cellViewModels[row][col].CanBeMovedTo = false;
                    cellViewModels[row][col].IsSelected = false;
                    cellViewModels[row][col].IsInCheck = false;
                }
            }
        }

        private void MarkWhichPiecesCanBeSelected()
        {
            var oppositeColor = Board.TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (var piece in Board.Pieces[Board.TurnColor])
            {
                var currCellViewModel = CellViewModels[piece.Cell.Row][piece.Cell.Col];
                currCellViewModel.CanBeSelected = true;
            }
            foreach (var piece in Board.Pieces[oppositeColor])
            {
                var currCellViewModel = CellViewModels[piece.Cell.Row][piece.Cell.Col];
                currCellViewModel.CanBeSelected = false;
                if (piece.PieceType == PieceType.King && (piece as King).IsInCheck)
                {
                    currCellViewModel.IsInCheck = false;
                }
            }
        }

        private void UpdateCellViewModels()
        {
            if (Board.UndoneMove != null)
            {
                if (Board.UndoneMove.IsPromotionMove)
                {
                    UpdateCellViewModelOfUndoneMoveCells(Board.UndoneMove);
                }
                else
                {
                    UpdateCellViewModelOfUndoneMoveCells(Board.UndoneMove);
                }
                Board.UndoneMove = null;
            }
            if (backupCellsToUpdate.Count > 0)
            {
                UpdateCellViewModelsOfBackupCells();
            }
            else if (Board.Moves.Count > 0)
            {
                var lastMove = Board.Moves.Peek();
                UpdateCellViewModelOfMoveCells(lastMove);
            }
        }

        private void UpdateCellViewModelOfMoveCells(Move move)
        {
            if (!move.IsPromotionMove)
            {
                CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].Cell = Board.Cells[move.CellOneBefore.Row, move.CellOneBefore.Col];
            }
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].UpdateCellImage();
            CellViewModels[move.CellTwoBefore.Row][move.CellTwoBefore.Col].Cell = Board.Cells[move.CellTwoBefore.Row, move.CellTwoBefore.Col];
            CellViewModels[move.CellTwoBefore.Row][move.CellTwoBefore.Col].UpdateCellImage();
            if (move.CellThreeBefore != null)
            {
                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].Cell = Board.Cells[move.CellThreeBefore.Row, move.CellThreeBefore.Col];
                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].UpdateCellImage();
            }
            if (move.CellFourBefore != null)
            {
                CellViewModels[move.CellFourBefore.Row][move.CellFourBefore.Col].Cell = Board.Cells[move.CellFourBefore.Row, move.CellFourBefore.Col];
                CellViewModels[move.CellFourBefore.Row][move.CellFourBefore.Col].UpdateCellImage();
            }
        }

        private void UpdateCellViewModelOfUndoneMoveCells(Move move)
        {
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].Cell = Board.Cells[move.CellOneBefore.Row, move.CellOneBefore.Col];
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].Cell.Piece = Board.Pieces[move.CellOneBefore.Piece.Color].First(p => p.Equals(move.CellOneBefore.Piece));
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].UpdateCellImage();
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].CanBeSelectedForPromotion = false;
            CellViewModels[move.CellTwoBefore.Row][move.CellTwoBefore.Col].Cell = Board.Cells[move.CellTwoBefore.Row, move.CellTwoBefore.Col];
            CellViewModels[move.CellTwoBefore.Row][move.CellTwoBefore.Col].UpdateCellImage();
            if (move.CellThreeBefore != null)
            {
                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].Cell = Board.Cells[move.CellThreeBefore.Row, move.CellThreeBefore.Col];
                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].Cell.Piece = Board.Pieces[move.CellThreeBefore.Piece.Color].First(p => p.Equals(move.CellThreeBefore.Piece));

                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].UpdateCellImage();
            }
            if (move.CellFourBefore != null)
            {
                CellViewModels[move.CellFourBefore.Row][move.CellFourBefore.Col].Cell = Board.Cells[move.CellFourBefore.Row, move.CellFourBefore.Col];
                CellViewModels[move.CellFourBefore.Row][move.CellFourBefore.Col].UpdateCellImage();
            }
        }

        private void MakeAllPiecesUnselectable()
        {
            var oppositeColor = Board.TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (var pieceCell in Board.Pieces[Board.TurnColor].Select(p => p.Cell))
            {
                CellViewModels[pieceCell.Row][pieceCell.Col].CanBeSelected = false;
            }
            foreach (var pieceCell in Board.Pieces[oppositeColor].Select(p => p.Cell))
                {
                CellViewModels[pieceCell.Row][pieceCell.Col].CanBeSelected = false;
                }
            }
        }

        private void UpdateCellViewModelsForPromotion()
        {
            CellViewModels[Board.PromotionMove.CellOneBefore.Row][Board.PromotionMove.CellOneBefore.Col].UpdateCellImage();
            CellViewModels[Board.PromotionMove.CellOneBefore.Row][Board.PromotionMove.CellOneBefore.Col].CanBeSelected = false;
            foreach (var cell in Board.BackupCells)
            {
                CellViewModels[cell.Row][cell.Col].Cell = Board.Cells[cell.Row, cell.Col];
                CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = true;
                cellViewModels[cell.Row][cell.Col].IsInCheck = false;
                CellViewModels[cell.Row][cell.Col].UpdateCellImage();
            }
        }

        private void FinishMove(Move move, Cell selectedCell)
        {
            Board.FinishMove(move, selectedCell);
            if (move.CellOneBefore.Piece.PieceType == PieceType.King && CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].IsInCheck)
            {
                CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].IsInCheck = false;
            }
            if (!GameHasStarted)
            {
                GameHasStarted = true;
            }
            PrepareForNextTurn();
        }

        private void RestoreBackupCells()
        {
            backupCellsToUpdate = Board.BackupCells.Where(c => c.Row != 7 && c.Row != 0).ToList();
            Board.RestoreBackupCells();
        }

        private void RestoreAllBackupCells()
        {
            backupCellsToUpdate = Board.BackupCells.ToList();
            Board.RestoreAllBackupCells();
            CellViewModels[Board.PromotionMove.CellOneBefore.Row][Board.PromotionMove.CellOneBefore.Col].CanBeSelected = true;
        }

        private void UpdateCellViewModelsOfBackupCells()
        {
            var promotionMove = Board.PromotionMove;
            CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].CanBeSelectedForPromotion = false;
            var pawnBeforePromotion = Board.Pieces[promotionMove.CellOneBefore.Piece.Color].FirstOrDefault(p => p.Equals(promotionMove.CellOneBefore.Piece));

            CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].Cell.Piece = pawnBeforePromotion;
            CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].UpdateCellImage();

            foreach (var backupCell in backupCellsToUpdate.Where(c => !c.HasEqualRowAndCol(promotionMove.CellOneBefore)))
            {
                CellViewModels[backupCell.Row][backupCell.Col].CanBeSelectedForPromotion = false;
                CellViewModels[backupCell.Row][backupCell.Col].Cell = Board.Cells[backupCell.Row, backupCell.Col];
                if (Board.Cells[backupCell.Row, backupCell.Col].Piece != null)
                {
                    CellViewModels[backupCell.Row][backupCell.Col].Cell.Piece = Board.Pieces[Board.Cells[backupCell.Row, backupCell.Col].Piece.Color]
                        .First(p => p.Cell.HasEqualRowAndCol(Board.Cells[backupCell.Row, backupCell.Col]));
                }
                CellViewModels[backupCell.Row][backupCell.Col].UpdateCellImage();
            }
            backupCellsToUpdate.Clear();
        }

        private void UpdateCellViewModelsOfBackupCellsOnUndoMove(Move move)
        {
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].CanBeSelectedForPromotion = false;
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].Cell.Piece = null;
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].UpdateCellImage();
        }
    }
}
