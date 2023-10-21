using ChessWPF.ViewModels;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class MoveCommand : CommandBase
    {
        private readonly CellViewModel cellViewModel;

        public MoveCommand(CellViewModel cellViewModel)
        {
            this.cellViewModel = cellViewModel;
            cellViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return cellViewModel.CanBeMovedTo;

        }

        public override void Execute(object? parameter)
        {
            cellViewModel.MoveToCell();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CellViewModel.CanBeMovedTo) || e.PropertyName == nameof(CellViewModel.CellImage) )
            {
                OnCanExecuteChanged();
            }
        }
    }
}
