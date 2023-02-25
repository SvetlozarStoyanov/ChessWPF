using ChessWPF.Singleton;
using ChessWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            BackgroundSingleton.Instance.SelectPieceForPromotion(cellViewModel);
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
