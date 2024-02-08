using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Options;
using ChessWPF.Models.Enums;
using System;
using System.Diagnostics;
using System.Windows.Threading;
using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Clocks
{
    public sealed class GameClock
    {
        private int increment;
        private bool isLowTime;
        private PieceColor color;
        private TimeSpan timeLeft;
        private TimeSpan startingTime;
        private TimeSpan timeElapsed;
        private DispatcherTimer timer;
        private Stopwatch watch;

        public GameClock(TimeControl timeControl, PieceColor color)
        {
            Increment = timeControl.ClockIncrement;
            this.color = color;
            this.startingTime = TimeSpan.FromSeconds(timeControl.ClockTime);
            timeLeft = this.startingTime;
            timer = new DispatcherTimer();

            timer.Interval = TimeSpan.FromMilliseconds(64);
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
            private set => increment = value;
        }

        public bool IsLowTime
        {
            get { return isLowTime; }
            set { isLowTime = value; }
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
            isLowTime = false;
        }

        public void AddTime(TimeSpan time)
        {
            timeLeft += time;
            if (timeLeft < startingTime / 10)
            {
                isLowTime = true;
            }
            else
            {
                isLowTime = false;
            }
        }

        public void AddIncrement()
        {
            timeLeft += TimeSpan.FromSeconds(increment);
            if (timeLeft < startingTime / 10)
            {
                isLowTime = true;
            }
            else
            {
                isLowTime = false;
            }
        }

        public void RemoveIncrement()
        {
            timeLeft -= TimeSpan.FromSeconds(increment);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            OnClockTick();
        }

        private void OnClockTick()
        {
            if (timeLeft - watch.Elapsed <= TimeSpan.FromSeconds(0))
            {
                OnTimeOut();
                return;
            }
            if (timeLeft - watch.Elapsed < startingTime / 10)
            {
                isLowTime = true;
            }
            else
            {
                isLowTime = false;
            }
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
