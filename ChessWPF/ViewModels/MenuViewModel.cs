using ChessWPF.Commands;
using ChessWPF.Singleton;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        public MenuViewModel()
        {
            ResetBoardCommand = new ResetBoardCommand(BackgroundSingleton.Instance.BoardViewModel);
            UndoMoveCommand = new UndoMoveCommand(BackgroundSingleton.Instance.BoardViewModel);
        }
        public ICommand ResetBoardCommand { get; set; }
        public ICommand UndoMoveCommand { get; set; }
    }
}
