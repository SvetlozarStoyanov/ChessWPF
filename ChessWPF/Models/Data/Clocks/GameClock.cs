using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Singleton;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace ChessWPF.Models.Data.Clocks
{
    public class GameClock
    {
        private int increment;
        private PieceColor color;

        private DispatcherTimer timer;
        private Stopwatch watch;
        private TimeSpan timeLeft;
        private TimeSpan startingTime;

        public TimeSpan StartingTime
        {
            get { return startingTime; }
            private set { startingTime = value; }
        }


        public GameClock(int increment, int startingTime, PieceColor color)
        {
            this.increment = increment;
            this.color = color;
            this.startingTime = TimeSpan.FromSeconds(startingTime);
            timeLeft = this.startingTime;
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);

        }

        public int Increment
        {
            get { return increment; }
            set { increment = value; }
        }

        public PieceColor Color
        {
            get { return color; }
            set { color = value; }
        }
        public TimeSpan TimeLeft
        {
            get { return timeLeft; }
            set { timeLeft = value; }
        }

        public void StartClock()
        {
            timer.Start();
        }

        public void StopClock()
        {
            timer.Stop();
        }

        public void ResetClock()
        {
            timer.Stop();
            timeLeft = startingTime;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            timeLeft -= TimeSpan.FromSeconds(1);
            BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(timeLeft);
            if (timeLeft == TimeSpan.FromSeconds(0))
            {
                timer.Stop();
                var oppositeColor = this.color == PieceColor.White ? PieceColor.Black : PieceColor.White;
                BackgroundSingleton.Instance.EndGameByTimeOut(oppositeColor);
            }
        }

    }
}
