using ChessWPF.ViewModels;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class UndoMoveCommand : CommandBase
    {
        private readonly BoardViewModel boardViewModel;

        public UndoMoveCommand(BoardViewModel boardViewModel)
        {
            this.boardViewModel = boardViewModel;
            boardViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return boardViewModel.Board.Moves.Count > 0 || boardViewModel.PromotionIsUnderway;
        }

        public override void Execute(object? parameter)
        {
            boardViewModel.OnMoveUndo();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardViewModel.GameHasStarted) || e.PropertyName == nameof(BoardViewModel.PromotionIsUnderway))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
