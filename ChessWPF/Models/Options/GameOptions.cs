using ChessWPF.Models.Enums;

namespace ChessWPF.Models.Options
{
    public class GameOptions
    {
        public GameOptions(TimeControl timeControl)
        {
            TimeControl = timeControl;
        }
        public GameOptions() : this(new TimeControl(300, 5, TimeControlType.Blitz))
        {

        }

        public TimeControl TimeControl { get; set; }
    }
}
