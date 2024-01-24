using ChessWPF.Stores;
using System;

namespace ChessWPF.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        private NavigationStore navigationStore;
        public MainViewModel(NavigationStore navigationStore)
        {
            this.navigationStore = navigationStore;
            navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        public ViewModelBase CurrentViewModel => navigationStore.CurrentViewModel;

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
