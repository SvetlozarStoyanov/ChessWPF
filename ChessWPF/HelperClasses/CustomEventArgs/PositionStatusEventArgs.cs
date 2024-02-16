using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class PositionStatusEventArgs : EventArgs
    {
        public PositionStatusEventArgs(string? errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string? ErrorMessage { get; init; }

    }
}
