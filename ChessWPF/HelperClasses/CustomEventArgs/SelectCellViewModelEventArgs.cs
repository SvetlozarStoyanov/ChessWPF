using ChessWPF.ViewModels;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class SelectCellViewModelEventArgs : EventArgs
    {
        public SelectCellViewModelEventArgs(CellViewModel cellViewModel)
        {
            CellViewModel = cellViewModel;
        }

        public CellViewModel CellViewModel { get; set; } = null!;
    }
}
