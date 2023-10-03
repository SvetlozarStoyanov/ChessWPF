using ChessWPF.Models.Data.Clocks;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.ComponentModel;

namespace ChessWPF.ViewModels
{
    public class GameClockViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string timeLeft;
        private GameClock gameClock;


        public GameClockViewModel(PieceColor color)
        {
            gameClock = new GameClock(3, 300, color);
        }


        public int Increment
        {
            get { return gameClock.Increment; }
            set { gameClock.Increment = value; }
        }
        public string TimeLeft
        {
            get => timeLeft;
            set
            {
                timeLeft = value;
                OnPropertyChanged(nameof(TimeLeft));
            }
        }

        public PieceColor Color
        {
            get { return gameClock.Color; }
            set { gameClock.Color = value; }
        }

        public TimeSpan TimeElapsed
        {
            get
            {
                return gameClock.TimeElapsed;
            }
            set
            {
                gameClock.TimeElapsed = value;
            }
        }

        public GameClock GameClock
        {
            get { return gameClock; }
        }

        public void UpdateClock(TimeSpan timeLeft)
        {
            if (timeLeft.TotalSeconds > 10)
            {
                this.TimeLeft = timeLeft.ToString(@"mm\:ss");
            }
            else
            {
                this.TimeLeft = timeLeft.ToString(@"mm\:ss\:f");
            }
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

        public void AddTime(TimeSpan time)
        {
            gameClock.AddTime(time);
        }

        public void AddIncrement()
        {
            gameClock.AddIncrement();
        }

        public void RemoveIncrement()
        {
            gameClock.RemoveIncrement();
        }


    }
}
