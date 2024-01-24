using ChessWPF.Commands;
using ChessWPF.Stores;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public MainMenuViewModel(NavigationStore navigationStore)
        {
            NavigateToGameCommand = new NavigateCommand<GameViewModel>(navigationStore, () => new GameViewModel(navigationStore));
        }
        public ICommand NavigateToGameCommand { get; init; }
    }
}
