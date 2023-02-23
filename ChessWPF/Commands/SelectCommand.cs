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
            if (cellViewModel.CanBeSelectedForPromotion)
            {
                return true;
            }
            else if (cellViewModel.CanBeSelected)
            {
                return true;
            }
            return cellViewModel.CanBeSelected || cellViewModel.CanBeSelectedForPromotion;
            //return false;
        }
        public override void Execute(object? parameter)
        {
            if (cellViewModel.CanBeSelectedForPromotion)
            {
                BackgroundSingleton.Instance.SelectPieceForPromotion(cellViewModel);
            }
            else
            {
                cellViewModel.IsSelected = true;
                BackgroundSingleton.Instance.SelectCell(cellViewModel);
            }

        }
        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CellViewModel.IsSelected) || e.PropertyName == nameof(CellViewModel.CanBeSelected) || e.PropertyName == nameof(CellViewModel.CanBeSelectedForPromotion))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
