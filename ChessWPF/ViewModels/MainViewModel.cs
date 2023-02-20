using ChessWPF.Singleton;

namespace ChessWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public GameViewModel GameViewModel { get; set; }
        public MainViewModel()
        {
            GameViewModel = new GameViewModel();
            
            BackgroundSingleton.Instance.BoardViewModel = new BoardViewModel();
            GameViewModel.BoardViewModel = BackgroundSingleton.Instance.BoardViewModel;
            GameViewModel.MenuViewModel = new MenuViewModel();
            BackgroundSingleton.Instance.StartGame();
        }
    }
}
