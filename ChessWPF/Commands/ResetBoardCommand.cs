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
            if (boardViewModel.GameHasStarted || boardViewModel.GameHasEnded || boardViewModel.PromotionIsUnderway)
            {
                return true;
            }
            return false;
        }

        public override void Execute(object? parameter)
        {
            boardViewModel.OnReset();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardViewModel.GameHasStarted) 
                || e.PropertyName == nameof(BoardViewModel.GameHasEnded)
                || e.PropertyName == nameof(BoardViewModel.PromotionIsUnderway))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
