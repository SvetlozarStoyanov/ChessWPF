using ChessWPF.Models.Data.Pieces.Enums;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class TimeOutEventArgs : EventArgs
    {
        public TimeOutEventArgs(PieceColor color)
        {
            Color = color;
        }

        public PieceColor Color { get; set; }
    }
}
