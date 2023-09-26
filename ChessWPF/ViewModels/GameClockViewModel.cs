using ChessWPF.Models.Data.Clocks;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.ComponentModel;

namespace ChessWPF.ViewModels
{
    public class GameClockViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private GameClock gameClock;
        private TimeSpan timeLeft;

        public GameClockViewModel(PieceColor color)
        {
            gameClock = new GameClock(3, 10, color);
        }

        public GameClock GameClock
        {
            get { return gameClock; }
        }

        public TimeSpan TimeLeft
        {
            get => timeLeft;
            set
            {
                timeLeft = value;
                OnPropertyChanged(nameof(TimeLeft));
            }
        }


        public void UpdateClock(TimeSpan timeLeft)
        {
            this.TimeLeft = timeLeft;
        }

        public void StartClock()
        {
            gameClock.StartClock();
        }

        public void StopClock()
        {
            gameClock.StopClock();
        }
        public void ResetClock()
        {
            gameClock.ResetClock();
        }
    }
}
