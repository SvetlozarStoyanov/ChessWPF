using ChessWPF.HelperClasses.CustomEventArgs;
﻿using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.ViewModels
{
    public sealed class BoardViewModel : ViewModelBase
    {
        private bool promotionIsUnderway;
        private Move promotionMove;
        private Move undoneMove;
        private CellViewModel? selectedCell;
        private Board board;
        private CellViewModel[][] cellViewModels;
        private List<Cell> backupCellsToUpdate;
        private List<Cell> legalMoves;

        public BoardViewModel()
        {
            Board = new Board();
            cellViewModels = new CellViewModel[8][];
            MatchCellViewModelsToCells();
            //Board.TurnColor = PieceColor.White;
            backupCellsToUpdate = new List<Cell>();
            PromotionIsUnderway = false;
        }

        public delegate void MoveEventHandler(object sender, MovedToCellViewModelEventArgs args);
        public event MoveEventHandler MovedPiece;

        public delegate void UndoMoveEventHandler(object sender, EventArgs args);
        public event UndoMoveEventHandler UndoLastMove;

        public delegate void ResetBoardEventHandler(object sender, EventArgs args);
        public event ResetBoardEventHandler Reset;

        public delegate void PromotePieceEventHandler(object sender, PromotePieceEventArgs args);
        public event PromotePieceEventHandler Promote;

        public bool PromotionIsUnderway
        {
            get => promotionIsUnderway;
            set
            {
                promotionIsUnderway = value;
                OnPropertyChanged(nameof(PromotionIsUnderway));
            }
        }

        public string GameResult
        {
            get => Board.GameResult;
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
            get => Board.GameHasEnded;
            set
            {
                Board.GameHasEnded = value;
                OnPropertyChanged(nameof(GameHasEnded));
            }
        }

        public bool GameHasStarted
        {
            get => Board.GameHasStarted;
            set
            {
                Board.GameHasStarted = value;
                OnPropertyChanged(nameof(GameHasStarted));
            }
        }

        public CellViewModel? SelectedCell
        {
            get => selectedCell;
            set => selectedCell = value;
        }

        public Board Board
        {
            get => board;
            private set
            {
                board = value;
                OnPropertyChanged(nameof(Board));
            }
        }

        public CellViewModel[][] CellViewModels
        {
            get => cellViewModels;
            set
            {
                cellViewModels = value;
                OnPropertyChanged(nameof(CellViewModels));
            }
        }

        public List<Cell> LegalMoves
        {
            get => legalMoves;
            private set => legalMoves = value;
        }

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
                PromotionIsUnderway = false;
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

            UpdateCellViewModels();
            var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);
            CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = false;
            Board.CalculatePossibleMoves();
            if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King).IsInCheck)
            {
                CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
            }

            if (Board.CheckForGameEnding())
            {
                MakeAllPiecesUnselectable();
                GameResult = Board.GameResult;
            }
            else
            {
                MarkWhichPiecesCanBeSelected();
            }

            FenAnnotation = Board.FenAnnotation;
        }

        public void MovePiece(Cell cell, Cell selectedCell)
        {
            var move = Board.MovePiece(cell, selectedCell);
            if (Board.PromotionMove != null)
            {
                promotionMove = Board.PromotionMove;
                MakeAllPiecesUnselectable();
                UpdateCellViewModelsForPromotion();
                PromotionIsUnderway = true;
                promotionMove = null;
            }
            if (Board.BackupCells.Count == 0)
            {
                FinishMove(move, selectedCell);
            }
        }

        public void UndoMove()
        {
            if (Board.PromotionMove != null)
            {
                promotionMove = Board.PromotionMove;
                RestoreAllBackupCells();
                Board.UndoPromotionMove();
                UpdateCellViewModels();
                MarkWhichPiecesCanBeSelected();
                Board.CalculatePossibleMoves();
                var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);

                if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King).IsInCheck)
                {
                    CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
                }
                promotionMove = null;
                PromotionIsUnderway = false;
            }
            else
            {
                undoneMove = Board.Moves.Peek();
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
            CellViewModels[Board.PromotionMove.CellOneBefore.Row][Board.PromotionMove.CellOneBefore.Col].UpdateCellImage();
            FinishMove(Board.PromotionMove, null);
            PromotionIsUnderway = false;
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
                    cellViewModels[row][col].CanBeSelected = false;
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

            if (promotionMove != null)
            {
                CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].CanBeSelectedForPromotion = false;
                var pawnBeforePromotion = promotionMove.CellOneBefore.Piece;

                CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].Cell.Piece = pawnBeforePromotion;
                CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].UpdateCellImage();
            }

            foreach (var backupCell in backupCellsToUpdate.Where(c => promotionMove != null ? !c.HasEqualRowAndCol(promotionMove.CellOneBefore) : true))
            {
                CellViewModels[backupCell.Row][backupCell.Col].CanBeSelectedForPromotion = false;
                CellViewModels[backupCell.Row][backupCell.Col].Cell = Board.Cells[backupCell.Row, backupCell.Col];

                CellViewModels[backupCell.Row][backupCell.Col].UpdateCellImage();
            }
            backupCellsToUpdate.Clear();
        }

        private void UpdateCellViewModels()
        {
            if (Board.Moves.Any())
            {
                MakeCellViewModelsOfLastMoveUnselectable();
            }
            if (undoneMove != null)
            {
                UpdateCellViewModelOfUndoneMoveCells();
                this.undoneMove = null;
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
            var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);
            if (king.IsInCheck)
            {
                CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
            }
        }

        private void UpdateCellViewModelOfMoveCells(Move move)
        {
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].Cell = Board.Cells[move.CellOneBefore.Row, move.CellOneBefore.Col];
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

        private void UpdateCellViewModelOfUndoneMoveCells()
        {
            CellViewModels[undoneMove.CellOneBefore.Row][undoneMove.CellOneBefore.Col].Cell = Board.Cells[undoneMove.CellOneBefore.Row, undoneMove.CellOneBefore.Col];
            CellViewModels[undoneMove.CellOneBefore.Row][undoneMove.CellOneBefore.Col].UpdateCellImage();
            CellViewModels[undoneMove.CellOneBefore.Row][undoneMove.CellOneBefore.Col].CanBeSelectedForPromotion = false;

            CellViewModels[undoneMove.CellTwoBefore.Row][undoneMove.CellTwoBefore.Col].Cell = Board.Cells[undoneMove.CellTwoBefore.Row, undoneMove.CellTwoBefore.Col];
            CellViewModels[undoneMove.CellTwoBefore.Row][undoneMove.CellTwoBefore.Col].UpdateCellImage();

            if (undoneMove.CellThreeBefore != null)
            {
                CellViewModels[undoneMove.CellThreeBefore.Row][undoneMove.CellThreeBefore.Col].Cell = Board.Cells[undoneMove.CellThreeBefore.Row, undoneMove.CellThreeBefore.Col];
                CellViewModels[undoneMove.CellThreeBefore.Row][undoneMove.CellThreeBefore.Col].UpdateCellImage();
            }

            if (undoneMove.CellFourBefore != null)
            {
                CellViewModels[undoneMove.CellFourBefore.Row][undoneMove.CellFourBefore.Col].Cell = Board.Cells[undoneMove.CellFourBefore.Row, undoneMove.CellFourBefore.Col];
                CellViewModels[undoneMove.CellFourBefore.Row][undoneMove.CellFourBefore.Col].UpdateCellImage();
            }
        }

        private void UpdateCellViewModelsForPromotion()
        {
            if (!Board.BackupCells.Any(c => c.HasEqualRowAndCol(promotionMove.CellOneBefore)))
            {
                CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].Cell.Piece = null;
                CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].UpdateCellImage();
            }
            CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].CanBeSelected = false;
            foreach (var cell in Board.BackupCells)
            {
                CellViewModels[cell.Row][cell.Col].Cell = Board.Cells[cell.Row, cell.Col];
                CellViewModels[cell.Row][cell.Col].Cell.Piece = Board.Cells[cell.Row, cell.Col].Piece;
                CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = true;
                cellViewModels[cell.Row][cell.Col].IsInCheck = false;
                CellViewModels[cell.Row][cell.Col].UpdateCellImage();
            }
        }

        private void MakeCellViewModelsOfLastMoveUnselectable()
        {
            var lastMove = Board.Moves.Peek();
            CellViewModels[lastMove.CellOneBefore.Row][lastMove.CellOneBefore.Col].CanBeSelected = false;
            if (lastMove.CellThreeBefore != null)
            {
                CellViewModels[lastMove.CellThreeBefore.Row][lastMove.CellThreeBefore.Col].CanBeSelected = false;
            }
            if (lastMove.CellFourBefore != null)
            {
                CellViewModels[lastMove.CellFourBefore.Row][lastMove.CellFourBefore.Col].CanBeSelected = false;
            }
        }
    }
}
