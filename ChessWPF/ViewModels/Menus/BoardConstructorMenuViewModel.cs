using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class BoardConstructorMenuViewModel : ViewModelBase
    {
        public BoardConstructorMenuViewModel()
        {

        }

        public ICommand SelectPieceCommand { get; init; }
        public ICommand ClearBoardCommand { get; init; }
        public ICommand SetEnPassantSquareCommand { get; set; }
        public ICommand SetCastilingRightsCommand { get; set; }

    }
}
