using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.ViewModels;

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

        public Board Board 
        { 
            get => GameViewModel.BoardViewModel.Board;
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
