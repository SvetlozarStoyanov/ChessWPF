using ChessWPF.Models.Data.Pieces.Enums;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class PromotePieceEventArgs : EventArgs
    {
        public PromotePieceEventArgs(PieceType pieceType)
        {
            PieceType = pieceType;
        }

        public PieceType PieceType { get; }
    }
}
