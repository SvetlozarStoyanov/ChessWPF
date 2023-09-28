using ChessWPF.Commands;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class BoardViewModel : ViewModelBase
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
            Board.UpdateFenAnnotation();
            FenAnnotation = Board.FenAnnotation;
        }

        public void ResetBoard()
        {
            if (Board.BackupCells.Count > 0)
            {
                RestoreAllBackupCells();
                UpdateCellImagesOfBackupCells();
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
                //MakeAllPiecesUnselectable();
                return;
            }
            //Board.CheckForGameEnding();
            if (Board.BackupCells.Count > 0)
            {
                MakeAllPiecesUnselectable();
                return;
            }
            //Board.ReverseTurnColor();
            MarkWhichPiecesCanBeSelected();
            var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);
            CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = false;
            Board.CalculatePossibleMoves();
            if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King).IsInCheck)
            {
                CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
            }
            if (Board.CheckForGameEnding())
            {
                GameHasEnded = true;
            }
            //Board.UpdateFenAnnotation();
            //if (Board.Moves.Any())
            //{
            //    Board.Moves.Peek().FenAnnotation = Board.FenAnnotation;
            //}
            FenAnnotation = Board.FenAnnotation;
        }


        public void MovePiece(Cell cell, Cell selectedCell)
        {
            var move = Board.MovePiece(cell, selectedCell);
            if (Board.PromotionMove != null)
            {
                MakeAllPiecesUnselectable();
                MarkCellsForPromotion();
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
                RestoreAllBackupCells();
                Board.UndoPromotionMove();
                MarkWhichPiecesCanBeSelected();
                Board.CalculatePossibleMoves();
                var king = (King)Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King);

                if ((Board.Pieces[Board.TurnColor].First(p => p.PieceType == PieceType.King) as King).IsInCheck)
                {
                    CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
                }
                UpdateCellImagesOfBackupCells();

                Board.PromotionMove = null;
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
        }


        public void PromotePiece(PieceType pieceType)
        {
            Board.PromotePiece(pieceType);
            var cell = Board.PromotionMove.CellTwoAfter;
            //cell.Piece = PieceConstructor.ConstructPieceByType(pieceType, TurnColor, cell);
            CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = false;
            CellViewModels[cell.Row][cell.Col].UpdateCellImage();
            RestoreBackupCells();
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
                }
            }
        }

        private void MarkWhichPiecesCanBeSelected()
        {
            Board.Pieces[PieceColor.White] = new List<Piece>();
            Board.Pieces[PieceColor.Black] = new List<Piece>();
            var oppositeColor = Board.TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            for (int row = 0; row < CellViewModels.Length; row++)
            {
                for (int col = 0; col < CellViewModels.Length; col++)
                {
                    CellViewModels[row][col].Cell = Board.Cells[row, col];
                    var currCellViewModel = CellViewModels[row][col];

                    if (currCellViewModel.IsInCheck)
                    {
                        currCellViewModel.IsInCheck = false;
                    }
                    if (Board.Cells[row, col].Piece == null)
                    {
                        currCellViewModel.CanBeSelected = false;
                    }
                    else if (Board.Cells[row, col].Piece.Color == Board.TurnColor)
                    {
                        Board.Pieces[Board.TurnColor].Add(Board.Cells[row, col].Piece);
                        currCellViewModel.CanBeSelected = true;
                    }
                    else
                    {
                        Board.Pieces[oppositeColor].Add(Board.Cells[row, col].Piece);
                        currCellViewModel.CanBeSelected = false;
                    }
                    //currCellViewModel.UpdateCellImage();
                }

            }
            if (Board.UndoneMove != null)
            {
                UpdateCellImagesOfMove(Board.UndoneMove);
                Board.UndoneMove = null;
            }
            if (backupCellsToUpdate.Count > 0)
            {
                UpdateCellImagesOfBackupCells();
            }
            else if (Board.Moves.Count > 0)
            {
                var lastMove = Board.Moves.Peek();
                UpdateCellImagesOfMove(lastMove);
            }
        }

        private void UpdateCellImagesOfMove(Move move)
        {
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].UpdateCellImage();
            CellViewModels[move.CellTwoBefore.Row][move.CellTwoBefore.Col].UpdateCellImage();
            if (move.CellThreeBefore != null)
            {
                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].UpdateCellImage();
            }
            if (move.CellFourBefore != null)
            {
                CellViewModels[move.CellFourBefore.Row][move.CellFourBefore.Col].UpdateCellImage();
            }
        }

        private void MakeAllPiecesUnselectable()
        {
            foreach (var cellViewModelRow in CellViewModels)
            {
                foreach (var cellViewModel in cellViewModelRow)
                {
                    cellViewModel.CanBeSelected = false;
                }
            }
        }

        private void MarkCellsForPromotion()
        {
            CellViewModels[Board.PromotionMove.CellOneBefore.Row][Board.PromotionMove.CellOneBefore.Col].UpdateCellImage();
            foreach (var cell in Board.BackupCells)
            {
                CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = true;
                cellViewModels[cell.Row][cell.Col].IsInCheck = false;
                CellViewModels[cell.Row][cell.Col].UpdateCellImage();
            }
        }

        private void FinishMove(Move move, Cell selectedCell)
        {
            Board.FinishMove(move, selectedCell);
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

        private void UpdateCellImagesOfBackupCells()
        {
            var promotionMove = Board.PromotionMove;
            CellViewModels[promotionMove.CellOneBefore.Row][promotionMove.CellOneBefore.Col].UpdateCellImage();

            foreach (var backupCell in backupCellsToUpdate)
            {
                CellViewModels[backupCell.Row][backupCell.Col].CanBeSelectedForPromotion = false;
                CellViewModels[backupCell.Row][backupCell.Col].UpdateCellImage();
            }
            backupCellsToUpdate.Clear();
        }
    }
}
