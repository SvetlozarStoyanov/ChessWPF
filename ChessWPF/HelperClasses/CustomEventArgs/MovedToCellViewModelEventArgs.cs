using ChessWPF.Models.Data.Board;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class MovedToCellViewModelEventArgs : EventArgs
    {
        public MovedToCellViewModelEventArgs(Cell cell)
        {
            this.Cell = cell;
        }

        public Cell Cell { get; } = null!;
    }
}
