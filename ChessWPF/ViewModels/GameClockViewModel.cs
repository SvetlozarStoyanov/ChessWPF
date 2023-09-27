using ChessWPF.Models.Data.Clocks;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.ComponentModel;

namespace ChessWPF.ViewModels
{
    public class GameClockViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private GameClock gameClock;
        private string timeLeft;

        public GameClockViewModel(PieceColor color)
        {
            gameClock = new GameClock(3, 10, color);
        }

        public GameClock GameClock
        {
            get { return gameClock; }
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

        public void AddIncrement()
        {
            gameClock.AddIncrement();
        }
    }
}
