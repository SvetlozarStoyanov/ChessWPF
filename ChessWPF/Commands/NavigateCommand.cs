using ChessWPF.Stores;
using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Commands
{
    public class NavigateCommand<TViewModel> : CommandBase 
        where TViewModel : ViewModelBase
    {
        private GameStateStore gameStateStore;
        private readonly Func<TViewModel> createViewModel;

        public NavigateCommand(GameStateStore gameStateStore, Func<TViewModel> createViewModel)
        {
            this.gameStateStore = gameStateStore;
            this.createViewModel = createViewModel;
        }

        public override void Execute(object? parameter)
        {
            gameStateStore.CurrentViewModel = createViewModel();
        }
    }
}
