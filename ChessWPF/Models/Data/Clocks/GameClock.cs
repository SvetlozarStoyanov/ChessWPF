using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace ChessWPF.Models.Data.Clocks
{
    public sealed class GameClock
    {
        private int increment;
        private PieceColor color;
        private TimeSpan timeLeft;
        private TimeSpan startingTime;
        private TimeSpan timeElapsed;
        private DispatcherTimer timer;
        private Stopwatch watch;

        public GameClock(int increment, int startingTime, PieceColor color)
        {
            this.increment = increment;
            this.color = color;
            this.startingTime = TimeSpan.FromSeconds(startingTime);
            timeLeft = this.startingTime;
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += Timer_Tick;
            watch = new Stopwatch();
        }

        public delegate void ClockTickEventHandler(object source, ClockTickEventArgs args);
        public delegate void TimeOutEventHandler(object source, TimeOutEventArgs args);
        public event ClockTickEventHandler ClockTick;
        public event TimeOutEventHandler TimeOut;

        public int Increment
        {
            get => increment;
            set => increment = value;
        }

        public PieceColor Color
        {
            get => color;
            private set => color = value;
        }

        public TimeSpan TimeLeft
        {
            get => timeLeft;
        }

        public TimeSpan StartingTime
        {
            get => startingTime;
            private set => startingTime = value;
        }

        public TimeSpan TimeElapsed
        {
            get => timeElapsed;
            set => timeElapsed = value;
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
        }

        public void AddIncrement()
        {
            timeLeft += TimeSpan.FromSeconds(increment);
        }

        public void RemoveIncrement()
        {
            timeLeft -= TimeSpan.FromSeconds(increment);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            OnClockTick();
            if (timeLeft - watch.Elapsed <= TimeSpan.FromSeconds(0))
            {
                OnTimeOut();
                return;
            }
        }

        private void OnClockTick()
        {
            ClockTick(this, new ClockTickEventArgs(timeLeft - watch.Elapsed));
        }

        private void OnTimeOut()
        {
            watch.Stop();
            timer.Stop();
            watch.Reset();
            TimeOut(this, new TimeOutEventArgs(color));
        }
    }
}
