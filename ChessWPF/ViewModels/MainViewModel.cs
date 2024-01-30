using ChessWPF.Stores;
using System;

namespace ChessWPF.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        private GameStateStore gameStateStore;
        public MainViewModel(GameStateStore navigationStore)
        {
            this.gameStateStore = navigationStore;
            navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        public ViewModelBase CurrentViewModel => gameStateStore.CurrentViewModel;

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
