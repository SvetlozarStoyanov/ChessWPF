using ChessWPF.Singleton;
using ChessWPF.ViewModels;
using System;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class SelectCommand : CommandBase
    {
        private readonly CellViewModel cellViewModel;
        public SelectCommand(CellViewModel cellViewModel)
        {
            this.cellViewModel = cellViewModel;
            cellViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }



        public override bool CanExecute(object? parameter)
        {
            return cellViewModel.CanBeSelected && base.CanExecute(parameter);
            //return false;
        }
        public override void Execute(object? parameter)
        {
            cellViewModel.IsSelected = true;
            BackgroundSingleton.Instance.SelectCell(cellViewModel);
            //BackgroundSingleton.Instance.GameBoardViewModel.CellsAreUpdated = false;
            //BackgroundSingleton.Instance.CellsAreUpdated = false;
        }
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CellViewModel.IsSelected) || e.PropertyName == nameof(CellViewModel.CellImage) || e.PropertyName == nameof(CellViewModel.CanBeSelected))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
