using ChessWPF.Game;
using ChessWPF.Models.Data.Options;
using ChessWPF.Models.Positions;
using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Stores
{
    public class GameStateStore
    {
        private ViewModelBase currentViewModel;
        private GameOptions gameOptions;
        private Position currentPosition;



        public GameStateStore()
        {
            GameOptions = new GameOptions();
            CurrentPosition = PositionCreator.CreateDefaultPosition();
        }

        public event Action CurrentViewModelChanged;
        public ViewModelBase CurrentViewModel 
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }


        public GameOptions GameOptions
        {
            get { return gameOptions; }
            set { gameOptions = value; }
        }

        public Position CurrentPosition 
        { 
            get => currentPosition;
            set => currentPosition = value;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
