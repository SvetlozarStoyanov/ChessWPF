using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;

namespace ChessWPF.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private BoardViewModel boardViewModel;
        private MenuViewModel menuViewModel;
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

        public Dictionary<string, GameClockViewModel> GameClocks
        {
            get { return gameClocks; }
            private set { gameClocks = value; }
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
            BoardViewModel.EndGameByTimeOut(color);
        }
    }
}
