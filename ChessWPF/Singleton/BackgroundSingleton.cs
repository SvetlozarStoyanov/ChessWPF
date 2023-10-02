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

        public BoardViewModel BoardViewModel
        {
            get { return GameViewModel.BoardViewModel; }
            set { GameViewModel.BoardViewModel = value; }
        }

        public Board Board 
        { 
            get => GameViewModel.BoardViewModel.Board;
            set => GameViewModel.BoardViewModel.Board = value;
        }

        public CellViewModel SelectedCell
        {
            get => GameViewModel.SelectedCell;
            set => GameViewModel.SelectedCell = value;
        }

        public void StartGame()
        {
            GameViewModel.StartGame();
        }

        public void ResetBoard()
        {
            GameViewModel.ResetBoard();
        }

        public void SelectCell(CellViewModel cellViewModel)
        {
            GameViewModel.SelectCell(cellViewModel);
        }

        public void MovePiece(Cell cell)
        {
            GameViewModel.MovePiece(cell);
        }

        public void UndoMove()
        {
            GameViewModel.UndoMove();
        }

        public void SelectPieceForPromotion(CellViewModel cellViewModel)
        {
            GameViewModel.SelectPieceForPromotion(cellViewModel);
        }

        public void EndGameByTimeOut(PieceColor color)
        {
            GameViewModel.EndGameByTimeOut(color);
        }

    }
}
