using ChessWPF.Singleton;

namespace ChessWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public GameViewModel GameViewModel { get; set; }
        public MainViewModel()
        {
            BoardViewModel boardViewModel = new BoardViewModel();
            MenuViewModel menuViewModel = new MenuViewModel(boardViewModel);
            GameViewModel = new GameViewModel(boardViewModel, menuViewModel);
            BackgroundSingleton.Instance.GameViewModel = GameViewModel;
            BackgroundSingleton.Instance.StartGame();
        }
    }
}
