namespace ChessWPF.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private BoardViewModel boardViewModel;
        private MenuViewModel menuViewModel;

        public GameViewModel()
        {

        }


        public BoardViewModel BoardViewModel
        {
            get { return boardViewModel; }
            set
            {
                boardViewModel = value;
                OnPropertyChanged(nameof(BoardViewModel));
            }
        }
        public MenuViewModel MenuViewModel
        {
            get { return menuViewModel; }
            set { menuViewModel = value; }
        }
    }
}
