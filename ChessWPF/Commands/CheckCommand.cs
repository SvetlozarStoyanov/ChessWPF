using ChessWPF.ViewModels;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class CheckCommand : CommandBase
    {
        private readonly CellViewModel cellViewModel;

        public CheckCommand(CellViewModel cellViewModel)
        {
            this.cellViewModel = cellViewModel;
            cellViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return cellViewModel.IsInCheck;
        }

        public override void Execute(object? parameter)
        {
            
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CellViewModel.IsInCheck))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
