using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChessWPF.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private string moveNotation;
        private bool canCopyMoveNotation;
        private const string regexPattern = @"[0-9]{1}[\/]{0,1}[0-9]{0,1}-[0-9]{1}[\/]{0,1}[0-9]{0,1}";
        private Regex regex = new Regex(regexPattern);

        private CellViewModel selectedCell;
        private BoardViewModel boardViewModel;
        private MenuViewModel menuViewModel;
        private List<Cell> legalMoves = new List<Cell>();
        private Dictionary<string, GameClockViewModel> gameClocks;

        public GameViewModel(BoardViewModel boardViewModel,
            MenuViewModel menuViewModel,
            Dictionary<string, GameClockViewModel> gameClocks
            )
        {
            BoardViewModel = boardViewModel;
            MenuViewModel = menuViewModel;
            GameClocks = gameClocks;
        }

        public string MoveNotation
        {
            get
            {
                return moveNotation;
            }
            set
            {
                moveNotation = value;
                OnPropertyChanged(nameof(MoveNotation));
            }
        }


        public bool CanCopyMoveNotation
        {
            get
            {
                return canCopyMoveNotation;
            }
            set
            {
                canCopyMoveNotation = value;
                OnPropertyChanged(nameof(CanCopyMoveNotation));
            }
        }

        public BoardViewModel BoardViewModel
        {
            get { return boardViewModel; }
            set
            {
                boardViewModel = value;
                //OnPropertyChanged(nameof(BoardViewModel));
            }
        }

        public MenuViewModel MenuViewModel
        {
            get { return menuViewModel; }
            set { menuViewModel = value; }
        }

        public Board Board
        {
            get => BoardViewModel.Board;
            set => BoardViewModel.Board = value;
        }
        public CellViewModel SelectedCell { get => selectedCell; set => selectedCell = value; }


        public List<Cell> LegalMoves
        {
            get { return legalMoves; }
        }

        public Dictionary<string, GameClockViewModel> GameClocks
        {
            get { return gameClocks; }
            private set { gameClocks = value; }
        }

        public void StartGame()
        {
            BoardViewModel.StartGame();
            GameClocks[BoardViewModel.Board.TurnColor.ToString()].StartClock();
            UpdateClocks();
            MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
        }

        public void ResetBoard()
        {
            UnselectSelectedCell();
            BoardViewModel.ResetBoard();
            ResetMoveAnnotation();
            MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
            ResetClocks();
            UpdateClocks();
            GameClocks[BoardViewModel.Board.TurnColor.ToString()].StartClock();
        }

        public void SelectCell(CellViewModel cellViewModel)
        {
            if (SelectedCell != null && cellViewModel.Cell.HasEqualRowAndCol(SelectedCell.Cell))
            {
                BoardViewModel.CellViewModels[SelectedCell.Cell.Row][SelectedCell.Cell.Col].IsSelected = false;
                UnselectSelectedCell();
                return;
            }
            SelectedCell = BoardViewModel.CellViewModels[cellViewModel.Cell.Row][cellViewModel.Cell.Col];
            SelectedCell.IsSelected = true;
            GetLegalMoves(SelectedCell.Cell.Piece);
        }


        public void MovePiece(Cell cell)
        {
            GameClocks[BoardViewModel.Board.TurnColor.ToString()].StopClock();
            ClearLegalMoves();

            GameClocks[BoardViewModel.Board.TurnColor.ToString()].AddIncrement();
            BoardViewModel.MovePiece(cell, SelectedCell.Cell);
            if (Board.Moves.Any() && !Board.Moves.Peek().IsPromotionMove && Board.PromotionMove == null)
            {
                AddToMoveAnnotation(Board.Moves.Peek());
            }
            SelectedCell = null;
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.GameHasEnded = true;
                MenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
                GameClocks[BoardViewModel.Board.TurnColor.ToString()].StartClock();
            }
        }

        public void UndoMove()
        {
            if (BoardViewModel.Board.PromotionMove == null)
            {
                GameClocks[BoardViewModel.Board.TurnColor.ToString()].StopClock();
                GameClocks[BoardViewModel.Board.TurnColor.ToString()].AddTime(GameClocks[BoardViewModel.Board.TurnColor.ToString()].TimeElapsed);
                RemoveFromMoveAnnotation();
            }

            if (SelectedCell != null)
            {
                UnselectSelectedCell();
            }
            BoardViewModel.UndoMove();
            MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
            GameClocks[BoardViewModel.Board.TurnColor.ToString()].StartClock();
        }

        public void SelectPieceForPromotion(CellViewModel cellViewModel)
        {
            GameClocks[BoardViewModel.Board.TurnColor.ToString()].StopClock();
            BoardViewModel.PromotePiece(cellViewModel.Cell.Piece.PieceType);
            AddToMoveAnnotation(Board.Moves.Peek());
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.GameHasEnded = true;
                MenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
                GameClocks[BoardViewModel.Board.TurnColor.ToString()].StartClock();
            }
        }

        public void ResetClocks()
        {
            foreach (var clock in gameClocks)
            {
                clock.Value.ResetClock();
            }
        }

        public void UpdateClocks()
        {
            foreach (var clock in gameClocks)
            {
                clock.Value.UpdateClock(clock.Value.GameClock.StartingTime);
            }
        }

        public void EndGameByTimeOut(PieceColor color)
        {
            MenuViewModel.UpdateGameStatus($"{color.ToString()} wins by timeout!");
            if (BoardViewModel.Board.PromotionMove != null)
            {
                BoardViewModel.UndoMove();
            }
            BoardViewModel.EndGameByTimeOut(color);
            UnselectSelectedCell();
            ClearLegalMoves();
        }

        private void GetLegalMoves(Piece piece)
        {
            var newLegalMoves = piece.LegalMoves;
            if (newLegalMoves != legalMoves)
            {
                ClearLegalMoves();
                ShowNewLegalMoves(newLegalMoves);
            }
        }

        private void ShowNewLegalMoves(List<Cell> newLegalMoves)
        {
            legalMoves = newLegalMoves;

            foreach (var cell in newLegalMoves)
            {
                BoardViewModel.CellViewModels[cell.Row][cell.Col].CanBeMovedTo = true;
            }
        }

        private void ClearLegalMoves()
        {
            foreach (var cell in legalMoves)
            {
                BoardViewModel.CellViewModels[cell.Row][cell.Col].CanBeMovedTo = false;
            }
            legalMoves = new List<Cell>();
        }

        private void UnselectSelectedCell()
        {
            SelectedCell = null;
            if (LegalMoves.Any())
            {
                ClearLegalMoves();
            }
        }

        private void AddToMoveAnnotation(Move move)
        {
            var sb = new StringBuilder(MoveNotation);
            sb.Append($"{(move.CellOneBefore.Piece.Color == PieceColor.White ?
                ($"{Board.Moves.Count(m => m.CellOneBefore.Piece.Color == PieceColor.White) + 1 / 2}.")
                : string.Empty)}{move.Annotation} ");
            MoveNotation = sb.ToString();
            CanCopyMoveNotation = true;

        }

        private void RemoveFromMoveAnnotation()
        {
            var sb = new StringBuilder(MoveNotation.TrimEnd());
            var lastSpaceIndex = MoveNotation.TrimEnd().LastIndexOf(" ");
            if (lastSpaceIndex == -1)
            {
                ResetMoveAnnotation();
                return;
            }
            var temp = MoveNotation.Substring(lastSpaceIndex, sb.Length - lastSpaceIndex);
            var match = regex.Match(temp);
            if (match.Success)
            {
                sb = sb.Remove(lastSpaceIndex, sb.Length - lastSpaceIndex);
                MoveNotation = sb.Append(" ").ToString();
            }
            lastSpaceIndex = MoveNotation.TrimEnd().LastIndexOf(" ");

            if (lastSpaceIndex == -1)
            {
                ResetMoveAnnotation();
                return;
            }

            sb = sb.Remove(lastSpaceIndex, sb.Length - lastSpaceIndex);
            MoveNotation = sb.Append(" ").ToString();
        }

        private void ResetMoveAnnotation()
        {
            MoveNotation = string.Empty;
            CanCopyMoveNotation = false;

        }
    }
}
