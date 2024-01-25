using ChessWPF.Commands;
using ChessWPF.Stores;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public MainMenuViewModel(GameStateStore gameStateStore)
        {
            NavigateToGameCommand = new NavigateCommand<GameViewModel>(gameStateStore, () => new GameViewModel(gameStateStore));
            NavigateToOptionsCommand = new NavigateCommand<GameOptionsViewModel>(gameStateStore, () => new GameOptionsViewModel(gameStateStore));
        }
        public ICommand NavigateToGameCommand { get; init; }
        public ICommand NavigateToOptionsCommand{ get; init; }
    }
}
