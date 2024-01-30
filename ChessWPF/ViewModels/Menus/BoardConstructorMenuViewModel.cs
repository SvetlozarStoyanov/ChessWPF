using ChessWPF.Stores;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class BoardConstructorMenuViewModel : ViewModelBase
    {
        public BoardConstructorMenuViewModel(GameStateStore gameStateStore)
        {
            
        }
        public ICommand NavigateToMainMenuCommand { get; init; }
        public ICommand NavigateToGameCommand { get; init; }
    }
}
