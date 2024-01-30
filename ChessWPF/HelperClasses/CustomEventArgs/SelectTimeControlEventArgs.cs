using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class SelectTimeControlEventArgs : EventArgs
    {
        public SelectTimeControlEventArgs(byte id)
        {
            Id = id;
        }

        public byte Id { get; set; }
    }
}
