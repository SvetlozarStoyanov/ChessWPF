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
    }
}
