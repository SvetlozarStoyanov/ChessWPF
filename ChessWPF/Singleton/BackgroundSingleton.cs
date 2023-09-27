using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Singleton
{
    public sealed class BackgroundSingleton
    {
        private static BackgroundSingleton instance = null;
        private static readonly object padlock = new object();
        private List<Cell> legalMoves = new List<Cell>();
        private CellViewModel selectedCell;
        private GameViewModel gameViewModel;

        public BackgroundSingleton()
        {

        }

        public static BackgroundSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BackgroundSingleton();
                    }
                    return instance;
                }
            }
        }
        public GameViewModel GameViewModel
        {
            get { return gameViewModel; }
            set { gameViewModel = value; }
        }

        public MenuViewModel MenuViewModel
        {
            get { return GameViewModel.MenuViewModel; }
            set { GameViewModel.MenuViewModel = value; }
        }

        public BoardViewModel BoardViewModel
        {
            get { return GameViewModel.BoardViewModel; }
            set { GameViewModel.BoardViewModel = value; }
        }
        public Board Board { get => BoardViewModel.Board; set => BoardViewModel.Board = value; }
        public List<Cell> LegalMoves
        {
            get { return legalMoves; }
        }
        public CellViewModel SelectedCell { get => selectedCell; set => selectedCell = value; }

        public void StartGame()
        {
            BoardViewModel.StartGame();
            GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StartClock();
            GameViewModel.UpdateClocks();
            MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
        }

        public void ResetBoard()
        {
            UnselectSelectedCell();
            BoardViewModel.ResetBoard();
            MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
            GameViewModel.ResetClocks();
            GameViewModel.UpdateClocks();
            GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StartClock();
        }

        public void SelectCell(CellViewModel cellViewModel)
        {
            SelectedCell = BoardViewModel.CellViewModels[cellViewModel.Cell.Row][cellViewModel.Cell.Col];
            GetLegalMoves(SelectedCell.Cell.Piece);
        }

        public void MovePiece(Cell cell)
        {
            GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StopClock();
            GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].AddIncrement();
            ClearLegalMoves();
            BoardViewModel.MovePiece(cell, SelectedCell.Cell);
            SelectedCell = null;
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.GameHasEnded = true;
                MenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");

                GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StartClock();
            }
        }

        public void UndoMove()
        {
            if (BoardViewModel.Board.PromotionMove == null)
            {
                GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StopClock();
            }
            if (SelectedCell != null)
            {
                UnselectSelectedCell();
            }
            BoardViewModel.UndoMove();
            MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
            GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StartClock();

        }

        public void SelectPieceForPromotion(CellViewModel cellViewModel)
        {
            GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StopClock();
            BoardViewModel.PromotePiece(cellViewModel.Cell.Piece.PieceType);
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.GameHasEnded = true;
                MenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                MenuViewModel.UpdateGameStatus($"{BoardViewModel.Board.TurnColor} to play");
                GameViewModel.GameClocks[BoardViewModel.Board.TurnColor.ToString()].GameClock.StartClock();
            }
        }

        public void EndGameByTimeOut(PieceColor color)
        {
            GameViewModel.EndGameByTimeOut(color);
            UnselectSelectedCell();
            ClearLegalMoves();
        }

        private void GetLegalMoves(Piece piece)
        {
            var newLegalMoves = SelectedCell.Cell.Piece.LegalMoves;
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
    }
}
