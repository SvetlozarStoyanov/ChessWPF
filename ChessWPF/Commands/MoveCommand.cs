using ChessWPF.Singleton;
using ChessWPF.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;

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

            return cellViewModel.CanBeMovedTo && BackgroundSingleton.Instance.SelectedCell != null;
            //return cellViewModel.CellIsUpdated;
            //return true;
        }
        public override void Execute(object? parameter)
        {
            BackgroundSingleton.Instance.MovePiece(cellViewModel.Cell);
            //cellViewModel.UpdateCellImage();
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
