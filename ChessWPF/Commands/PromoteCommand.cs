using ChessWPF.ViewModels;
using System.ComponentModel;

namespace ChessWPF.Commands
{
    public class PromoteCommand : CommandBase
    {
        private readonly CellViewModel cellViewModel;

        public PromoteCommand(CellViewModel cellViewModel)
        {
            this.cellViewModel = cellViewModel;
            cellViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return cellViewModel.CanBeSelectedForPromotion;
        }

        public override void Execute(object? parameter)
        {
            cellViewModel.OnPromotedTo();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CellViewModel.CanBeSelectedForPromotion))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
