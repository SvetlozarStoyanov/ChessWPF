using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Clocks;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace ChessWPF.ViewModels
{
    public sealed class GameClockViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string timeLeft;
        private bool isRunning;
        private GameClock gameClock;
        private SolidColorBrush clockBrush;
        private Color normalRunningColor;
        private Color stoppedColor;
        private Color lowTimeColor;

        public GameClockViewModel(PieceColor color,
            Color normalRunningColor,
            Color stoppedColor,
            Color lowTimeColor)
        {
            gameClock = new GameClock(3, 10, color);
            gameClock.ClockTick += OnClockTick;
            gameClock.TimeOut += OnTimeOut;
            NormalRunningColor = normalRunningColor;
            StoppedColor = stoppedColor;
            LowTimeColor = lowTimeColor;
            ClockBrush = new SolidColorBrush(StoppedColor);
            ClockBrush.Opacity = 0.6;
        }

        public delegate void TimeOutEventHandler(object source, TimeOutEventArgs args);

        public event TimeOutEventHandler TimeOut;

        public int Increment
        {
            get => gameClock.Increment;
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

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public PieceColor Color
        {
            get => gameClock.Color;
        }

        public TimeSpan TimeElapsed
        {
            get
            {
                return gameClock.TimeElapsed;
            }
            private set
            {
                gameClock.TimeElapsed = value;
            }
        }
        public Color NormalRunningColor
        {
            get => normalRunningColor;
            private set { normalRunningColor = value; }
        }

        public Color StoppedColor
        {
            get => stoppedColor;
            private set { stoppedColor = value; }
        }

        public Color LowTimeColor
        {
            get => lowTimeColor;
            private set { lowTimeColor = value; }
        }

        public GameClock GameClock
        {
            get { return gameClock; }
        }

        public SolidColorBrush ClockBrush
        {
            get => clockBrush;
            private set => clockBrush = value;
        }

        public void UpdateClock(TimeSpan timeLeft)
        {
            if (gameClock.IsLowTime)
            {
                ClockBrush.Color = LowTimeColor;
            }

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

            ClockBrush.Color = NormalRunningColor;

            IsRunning = true;
        }

        public void StopClock()
        {
            gameClock.StopClock();
            UpdateClock(gameClock.TimeLeft);

            ClockBrush.Color = StoppedColor;

            IsRunning = false;
        }

        public void ResetClock()
        {
            gameClock.ResetClock();
            UpdateClock(gameClock.TimeLeft);
            //IsRunning = false;
        }

        public void AddTime(TimeSpan time)
        {
            gameClock.AddTime(time);
            UpdateClock(gameClock.TimeLeft);
        }

        public void AddIncrement()
        {
            gameClock.AddIncrement();
            UpdateClock(gameClock.TimeLeft);
        }

        public void RemoveIncrement()
        {
            gameClock.RemoveIncrement();
            UpdateClock(gameClock.TimeLeft);
        }


        private void OnClockTick(object source, ClockTickEventArgs args)
        {
            UpdateClock(args.TimeLeft);
        }

        private void OnTimeOut(object source, TimeOutEventArgs args)
        {
            UpdateClock(TimeSpan.FromSeconds(0));
            IsRunning = false;
            TimeOut(this, new TimeOutEventArgs(this.Color));
        }
    }
}
