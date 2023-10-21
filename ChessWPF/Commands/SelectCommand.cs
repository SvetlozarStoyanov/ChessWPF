using ChessWPF.ViewModels;
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
            return cellViewModel.CanBeSelected;
        }

        public override void Execute(object? parameter)
        {
            cellViewModel.SelectCell();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CellViewModel.IsSelected) || e.PropertyName == nameof(CellViewModel.CanBeSelected))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
