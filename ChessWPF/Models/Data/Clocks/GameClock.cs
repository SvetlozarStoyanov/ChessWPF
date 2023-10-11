using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Singleton;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace ChessWPF.Models.Data.Clocks
{
    public sealed class GameClock
    {
        private int increment;
        private PieceColor color;

        private DispatcherTimer timer;
        private Stopwatch watch;
        private TimeSpan timeLeft;
        private TimeSpan startingTime;
        private TimeSpan timeElapsed;

        public GameClock(int increment, int startingTime, PieceColor color)
        {
            this.increment = increment;
            this.color = color;
            this.startingTime = TimeSpan.FromSeconds(startingTime);
            timeLeft = this.startingTime;
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += new EventHandler(Timer_Tick);
            watch = new Stopwatch();
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

        public TimeSpan StartingTime
        {
            get { return startingTime; }
            private set { startingTime = value; }
        }

        public TimeSpan TimeElapsed
        {
            get { return timeElapsed; }
            set { timeElapsed = value; }
        }

        public void StartClock()
        {
            timer.Start();
            timeElapsed = TimeSpan.FromSeconds(0);
            watch.Start();
        }

        public void StopClock()
        {
            watch.Stop();
            timer.Stop();
            timeLeft -= watch.Elapsed;
            timeElapsed = watch.Elapsed;
            watch.Reset();
            BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(timeLeft);
        }

        public void ResetClock()
        {
            timer.Stop();
            watch.Stop();
            watch.Reset();
            timeLeft = startingTime;
        }


        public void AddTime(TimeSpan time)
        {
            timeLeft += time;
            BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(timeLeft);
        }

        public void AddIncrement()
        {
            timeLeft += TimeSpan.FromSeconds(increment);
            BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(timeLeft);
        }

        public void RemoveIncrement()
        {
            timeLeft -= TimeSpan.FromSeconds(increment);
            BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(timeLeft);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            //timeLeft -= TimeSpan.FromMilliseconds(100);

            if (timeLeft - watch.Elapsed <= TimeSpan.FromSeconds(0))
            {
                watch.Stop();
                timer.Stop();
                watch.Reset();
                var oppositeColor = this.color == PieceColor.White ? PieceColor.Black : PieceColor.White;
                BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(TimeSpan.FromSeconds(0));
                BackgroundSingleton.Instance.EndGameByTimeOut(oppositeColor);
            }
            else
            {
                BackgroundSingleton.Instance.GameViewModel.GameClocks[Color.ToString()].UpdateClock(timeLeft - watch.Elapsed);

            }
        }

    }
}
