using ChessWPF.Commands;
using System.ComponentModel;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public sealed class GameMenuViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string gameStatus;

        public GameMenuViewModel(BoardViewModel boardViewModel)
        {
            ResetBoardCommand = new ResetBoardCommand(boardViewModel);
            UndoMoveCommand = new UndoMoveCommand(boardViewModel);
        }

        public string GameStatus
        {
            get { return gameStatus; }
            set
            {
                gameStatus = value;
                OnPropertyChanged(nameof(GameStatus));
            }
        }

        public ICommand ResetBoardCommand { get; set; }
        public ICommand UndoMoveCommand { get; set; }

        public void UpdateGameStatus(string status)
        {
            GameStatus = status;
        }

    }
}
