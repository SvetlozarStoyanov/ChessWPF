using ChessWPF.Models.Pieces.Enums;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class TurnColorChangedEventArgs : EventArgs
    {
        public TurnColorChangedEventArgs(PieceColor turnColor)
        {
            TurnColor = turnColor;
        }

        public PieceColor TurnColor { get; init; }
    }
}
