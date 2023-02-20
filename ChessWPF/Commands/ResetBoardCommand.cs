using ChessWPF.Singleton;
using ChessWPF.ViewModels;
using System.ComponentModel;
using System.Linq;

namespace ChessWPF.Commands
{
    public class ResetBoardCommand : CommandBase
    {
        private BoardViewModel boardViewModel;
        public ResetBoardCommand(BoardViewModel boardViewModel)
        {
            this.boardViewModel = boardViewModel;
            //boardViewModel.PropertyChanged += OnViewModelPropertyChanged;
            boardViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }



        public override bool CanExecute(object? parameter)
        {
            //this.boardViewModel = BackgroundSingleton.Instance.BoardViewModel;
            return boardViewModel.Board.Moves.Count > 0;
            //return true;
        }
        public override void Execute(object? parameter)
        {
            BackgroundSingleton.Instance.ResetBoard();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //boardViewModel = BackgroundSingleton.Instance.BoardViewModel;

            if (e.PropertyName == nameof(BoardViewModel.GameHasStarted))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
