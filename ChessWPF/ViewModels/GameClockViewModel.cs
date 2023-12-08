using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Clocks;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.ComponentModel;

namespace ChessWPF.ViewModels
{
    public sealed class GameClockViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private string timeLeft;
        private bool isRunning;
        private GameClock gameClock;

        public GameClockViewModel(PieceColor color)
        {
            gameClock = new GameClock(3, 300, color);
            gameClock.ClockTick += OnClockTick;
            gameClock.TimeOut += OnTimeOut;
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
            IsRunning = true;
        }

        public void StopClock()
        {
            gameClock.StopClock();
            UpdateClock(gameClock.TimeLeft);
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
