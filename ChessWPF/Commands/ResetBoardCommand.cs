using ChessWPF.Singleton;
using ChessWPF.ViewModels;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class ResetBoardCommand : CommandBase
    {
        private BoardViewModel boardViewModel;

        public ResetBoardCommand(BoardViewModel boardViewModel)
        {
            this.boardViewModel = boardViewModel;
            boardViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }



        public override bool CanExecute(object? parameter)
        {
            if (boardViewModel.GameHasStarted || boardViewModel.GameHasEnded)
            {
                return true;
            }
            return false;
        }

        public override void Execute(object? parameter)
        {
            BackgroundSingleton.Instance.ResetBoard();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardViewModel.GameHasStarted) || e.PropertyName == nameof(BoardViewModel.GameHasEnded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
