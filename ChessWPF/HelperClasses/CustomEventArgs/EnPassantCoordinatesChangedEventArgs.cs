using ChessWPF.Models.Cells;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class EnPassantCoordinatesChangedEventArgs : EventArgs
    {
        public EnPassantCoordinatesChangedEventArgs(CellCoordinates? cellCoordinates)
        {
            CellCoordinates = cellCoordinates;
        }

        public EnPassantCoordinatesChangedEventArgs()
        {

        }

        public CellCoordinates? CellCoordinates { get; init; }
    }
}
