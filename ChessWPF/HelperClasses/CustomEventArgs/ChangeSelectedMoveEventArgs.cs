using ChessWPF.Models.Moves;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class ChangeSelectedMoveEventArgs : EventArgs
    {
        public ChangeSelectedMoveEventArgs(Move move)
        {
            Move = move;
        }

        public Move Move { get; set; }
    }
}
