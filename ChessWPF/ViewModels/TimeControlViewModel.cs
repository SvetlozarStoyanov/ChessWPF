using ChessWPF.Commands;
using ChessWPF.Models.Data.Options;
using System;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class TimeControlViewModel : ViewModelBase
    {
        public TimeControlViewModel(TimeControl timeControl)
        {
            TimeControl = timeControl;
            SelectTimeControlCommand = new SelectTimeControlCommand(this);
        }

        public event Action Select;
        public bool IsSelected { get; set; }

        public string TimeAsText { get => TimeControl.ToString()!; } 
        public TimeControl TimeControl { get; init; }
        public ICommand SelectTimeControlCommand { get; }

        public void SelectChanged()
        {
            this.IsSelected = true;
            Select?.Invoke();
        }
    }
}
