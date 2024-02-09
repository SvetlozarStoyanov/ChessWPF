using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class UpdateCastlingPossibilitiesEventArgs : EventArgs
    {
        public UpdateCastlingPossibilitiesEventArgs(bool[] castlingPossibilities)
        {
            CastlingPossibilites = castlingPossibilities;
        }

        public bool[] CastlingPossibilites { get; init; }
    }
}
