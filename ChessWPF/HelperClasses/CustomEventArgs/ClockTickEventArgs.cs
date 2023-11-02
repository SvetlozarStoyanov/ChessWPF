using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class ClockTickEventArgs : EventArgs
    {
        public ClockTickEventArgs(TimeSpan timeLeft)
        {
            TimeLeft = timeLeft;
        }
        public TimeSpan TimeLeft { get; set; }
    }
}
