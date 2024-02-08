using ChessWPF.Models.Pieces;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class UpdateCellEventArgs : EventArgs
    {
        public UpdateCellEventArgs(Piece? piece)
        {
            this.Piece = piece;
        }
        public Piece? Piece { get; }
    }
}
