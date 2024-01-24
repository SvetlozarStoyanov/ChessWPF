using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Stores
{
    public class NavigationStore
    {
        private ViewModelBase currentViewModel;
        public NavigationStore()
        {

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

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
