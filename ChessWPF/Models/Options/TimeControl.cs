using ChessWPF.HelperClasses.Exceptions;
using ChessWPF.Models.Enums;

namespace ChessWPF.Models.Options
{
    public class TimeControl
    {
        private ushort clockTime;
        private ushort clockIncrement;

        public TimeControl(ushort clockTime, ushort clockIncrement, TimeControlType timeControlType)
        {
            ClockTime = clockTime;
            ClockIncrement = clockIncrement;
            TimeControlType = timeControlType;
        }
        public TimeControl() : this(300, 5, TimeControlType.Blitz)
        {

        }

        public ushort ClockTime
        {
            get { return clockTime; }
            set
            {
                if (value >= 15 && value <= 7200)
                {
                    clockTime = value;
                }
                else
                {
                    throw new InvalidTimeControlException();
                }
            }
        }

        public ushort ClockIncrement
        {
            get => clockIncrement;
            set => clockIncrement = value;
        }

        public TimeControlType TimeControlType { get; set; }

        public override string ToString()
        {
            return $"{(ClockTime >= 60 ? ClockTime / 60 : $"0:{ClockTime}")} | {ClockIncrement}";
        }
    }
}
