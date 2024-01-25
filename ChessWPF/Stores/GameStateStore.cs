using ChessWPF.Models.Data.Options;
using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Stores
{
    public class GameStateStore
    {
        private ViewModelBase currentViewModel;
        private GameOptions gameOptions;

        public GameStateStore()
        {
            GameOptions = new GameOptions();
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

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
