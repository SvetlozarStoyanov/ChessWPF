using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.ViewModels
{
    public sealed class BoardViewModel : ViewModelBase
    {
        private bool promotionIsUnderway;
        private CellViewModel? selectedCell;
        private Board board;
        private CellViewModel[][] cellViewModels;
        private List<Cell> legalMoves;

        public BoardViewModel()
        {
            Board = new Board();
            CellViewModels = new CellViewModel[8][];
            MatchCellViewModelsToCells();
            Board.SetupPieces();
            LegalMoves = new List<Cell>();
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

        public string? GameResult
        {
            get => Board.GameResult;
            set
            {
                Board.GameResult = value!;
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
            private set => selectedCell = value;
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

        public List<Cell> LegalMoves
        {
            get => legalMoves;
            private set => legalMoves = value;
        }

        public CellViewModel[][] CellViewModels
        {
            get => cellViewModels;
            private set
            {
                cellViewModels = value;
                OnPropertyChanged(nameof(CellViewModels));
            }
        }

        public void MatchCellViewModelsToCells()
        {
            for (int row = 0; row < Board.Cells.GetLength(0); row++)
            {
                CellViewModels[row] = new CellViewModel[8];
                for (int col = 0; col < Board.Cells.GetLength(1); col++)
                {
                    MatchCellViewModelToCell(row, col);
                }
            }
        }

        public void StartGame()
        {
            PrepareForNextTurn();
            Board.UpdateFenAnnotation();
            FenAnnotation = Board.FenAnnotation;
        }

        public void ResetBoard()
        {
            UnselectSelectedCell();
            if (Board.BackupCells.Count > 0)
            {
                UpdateCellViewModelsOfBackupCells();
                RestoreAllBackupCells();
                PromotionIsUnderway = false;
            }
            Board.Reset();
            GameHasStarted = false;
            GameHasEnded = false;
            GameResult = null;
            ResetMatchCellViewModelsToCells();
            StartGame();
        }

        public void PrepareForNextTurn()
        {
            ClearLegalMoves();
            var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);
            CellViewModels[king.Row][king.Col].IsInCheck = false;
            Board.CalculatePossibleMoves();
            if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King)!.IsInCheck)
            {
                CellViewModels[king.Row][king.Col].IsInCheck = true;
            }


            if (Board.CheckForGameEnding())
            {
                GameResult = Board.GameResult;
            }
            else
            {
                MarkWhichPiecesCanBeSelected();
            }

            FenAnnotation = Board.FenAnnotation;
        }

        public void MovePiece(Cell cell)
        {
            var move = Board.MovePiece(cell, SelectedCell!.Cell);
            if (Board.OngoingPromotionMove != null)
            {
                ClearLegalMoves();
                MakeAllPiecesUnselectable();
                PromotionIsUnderway = true;
            }
            if (Board.BackupCells.Count == 0)
            {
                FinishMove(move, SelectedCell.Cell);
            }
            UnselectSelectedCell();
        }

        public void UndoMove()
        {
            if (Board.OngoingPromotionMove != null)
            {
                UpdateCellViewModelsOfBackupCells();
                RestoreAllBackupCells();
                Board.UndoOngoingPromotionMove();
                MarkWhichPiecesCanBeSelected();
                Board.CalculatePossibleMoves();
                var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);

                if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King)!.IsInCheck)
                {
                    CellViewModels[king.Row][king.Col].IsInCheck = true;
                }
                PromotionIsUnderway = false;
            }
            else
            {
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
            UnselectSelectedCell();
        }

        public void PromotePiece(PieceType pieceType)
        {
            Board.PromotePiece(pieceType);
            var cell = Board.OngoingPromotionMove!.CellTwoAfter;
            CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = false;
            UpdateCellViewModelsOfBackupCells();
            RestoreBackupCells();
            FinishMove(Board.OngoingPromotionMove, null);
            PromotionIsUnderway = false;
        }

        public void EndGameByTimeOut(PieceColor color)
        {
            UnselectSelectedCell();
            ClearLegalMoves();
            if (Board.OngoingPromotionMove != null)
            {
                UndoMove();
            }
            GameResult = $"{color.ToString()} wins by timeout!";
            this.GameHasEnded = true;
            MakeAllPiecesUnselectable();
        }

        public void EndGame()
        {
            UnselectSelectedCell();
            ClearLegalMoves();

            this.GameHasEnded = true;
            MakeAllPiecesUnselectable();
        }

        public void OnMoveUndo()
        {
            UndoLastMove(this, EventArgs.Empty);
        }

        public void OnReset()
        {
            Reset(this, EventArgs.Empty);
        }

        public void OnPromote(PieceType pieceType)
        {
            Promote(this, new PromotePieceEventArgs(pieceType));
        }

        private void UnselectSelectedCell()
        {
            SelectedCell = null;
        }

        private List<Cell> GetLegalMoves(Piece piece)
        {
            var newLegalMoves = new List<Cell>(piece.LegalMoves);
            return newLegalMoves;
        }

        private void ShowLegalMoves()
        {
            foreach (var cell in legalMoves)
            {
                CellViewModels[cell.Row][cell.Col].CanBeMovedTo = true;
            }
        }

        private void ClearLegalMoves()
        {
            foreach (var cell in LegalMoves)
            {
                CellViewModels[cell.Row][cell.Col].CanBeMovedTo = false;
            }
            LegalMoves.Clear();
        }

        private void OnCellViewModelSelect(object sender, SelectCellViewModelEventArgs args)
        {
            ClearLegalMoves();
            var cellViewModel = args.CellViewModel;
            if (SelectedCell == null || !SelectedCell.Cell.HasEqualRowAndCol(args.CellViewModel.Cell))
            {
                SelectedCell = cellViewModel;
                LegalMoves = GetLegalMoves(cellViewModel.Cell.Piece!);
                ShowLegalMoves();
            }
            else
            {
                UnselectSelectedCell();
            }
        }

        private void OnCellViewModelMovedTo(object sender, MovedToCellViewModelEventArgs args)
        {
            MovedPiece(sender, args);
        }

        private void OnCellViewModelPromotedTo(object sender, PromotePieceEventArgs args)
        {
            OnPromote(args.PieceType);
        }

        private void MatchCellViewModelToCell(int row, int col)
        {
            CellViewModels[row][col] = new CellViewModel(Board.Cells[row, col]);
            var cellViewModel = CellViewModels[row][col];
            cellViewModel.Select += OnCellViewModelSelect;
            cellViewModel.MovedTo += OnCellViewModelMovedTo;
            cellViewModel.PromotedTo += OnCellViewModelPromotedTo;
        }

        private void ResetMatchCellViewModelsToCells()
        {
            for (int row = 0; row < Board.Cells.GetLength(0); row++)
            {
                for (int col = 0; col < Board.Cells.GetLength(1); col++)
                {
                    ResetMatchCellViewModelToCell(row, col);
                }
            }
        }

        private void ResetMatchCellViewModelToCell(int row, int col)
        {
            var cellViewModel = CellViewModels[row][col];
            cellViewModel.Cell = Board.Cells[row, col];
            
            cellViewModel.UpdateCellImage();
            cellViewModel.CanBeMovedTo = false;
            cellViewModel.CanBeSelected = false;
            cellViewModel.IsSelected = false;
            cellViewModel.IsInCheck = false;
        }

        private void MarkWhichPiecesCanBeSelected()
        {
            var oppositeColor = Board.TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (var piece in Board.Pieces[Board.TurnColor])
            {
                var currCellViewModel = CellViewModels[piece.Row][piece.Col];
                currCellViewModel.CanBeSelected = true;
            }
            foreach (var piece in Board.Pieces[oppositeColor])
            {
                var currCellViewModel = CellViewModels[piece.Row][piece.Col];
                currCellViewModel.CanBeSelected = false;
                if (piece.PieceType == PieceType.King && (piece as King)!.IsInCheck)
                {
                    currCellViewModel.IsInCheck = false;
                }
            }
        }

        private void MakeAllPiecesUnselectable()
        {
            var oppositeColor = Board.TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (var pieceCoordinates in Board.Pieces[Board.TurnColor].Select(p => new { Row = p.Row, Col = p.Col }))
            {
                CellViewModels[pieceCoordinates.Row][pieceCoordinates.Col].CanBeSelected = false;
            }
            foreach (var pieceCell in Board.Pieces[oppositeColor].Select(p => new { Row = p.Row, Col = p.Col }))
            {
                CellViewModels[pieceCell.Row][pieceCell.Col].CanBeSelected = false;
            }
        }

        private void FinishMove(Move move, Cell? selectedCell)
        {
            Board.FinishMove(move, selectedCell!);
            if (move.CellOneBefore.Piece!.PieceType == PieceType.King && CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].IsInCheck)
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
            Board.RestoreBackupCells();
        }

        private void RestoreAllBackupCells()
        {
            Board.RestoreAllBackupCells();
            CellViewModels[Board.OngoingPromotionMove!.CellOneBefore.Row][Board.OngoingPromotionMove.CellOneBefore.Col].CanBeSelected = true;
        }

        private void UpdateCellViewModelsOfBackupCells()
        {
            if (Board.OngoingPromotionMove != null)
            {
                CellViewModels[Board.OngoingPromotionMove.CellOneBefore.Row][Board.OngoingPromotionMove.CellOneBefore.Col].CanBeSelectedForPromotion = false;
            }

            foreach (var backupCell in Board.BackupCells.Where(c => Board.OngoingPromotionMove != null ? !c.HasEqualRowAndCol(Board.OngoingPromotionMove.CellOneBefore) : true))
            {
                CellViewModels[backupCell.Row][backupCell.Col].CanBeSelectedForPromotion = false;
            }
        }
    }
}
