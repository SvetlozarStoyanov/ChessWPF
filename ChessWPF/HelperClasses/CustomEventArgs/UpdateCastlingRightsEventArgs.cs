using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class UpdateCastlingRightsEventArgs : EventArgs
    {
        public UpdateCastlingRightsEventArgs(bool[] castlingRights)
        {
            CastlingRights = castlingRights;
        }
        public bool[] CastlingRights { get; init; }
    }
}
