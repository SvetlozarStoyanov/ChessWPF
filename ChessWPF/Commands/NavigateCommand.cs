using ChessWPF.Stores;
using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Commands
{
    public class NavigateCommand<TViewModel> : CommandBase 
        where TViewModel : ViewModelBase
    {
        private NavigationStore navigationStore;
        private readonly Func<TViewModel> createViewModel;

        public NavigateCommand(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            this.navigationStore = navigationStore;
            this.createViewModel = createViewModel;
        }

        public override void Execute(object? parameter)
        {
            navigationStore.CurrentViewModel = createViewModel();
        }
    }
}
