using ChessWPF.ViewModels;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class EndGameCommand : CommandBase
    {
        private readonly BoardViewModel boardViewModel;

        public EndGameCommand(BoardViewModel boardViewModel)
        {
            this.boardViewModel = boardViewModel;
            boardViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return boardViewModel.GameHasEnded;
        }

        public override void Execute(object? parameter)
        {

        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardViewModel.GameHasEnded))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
