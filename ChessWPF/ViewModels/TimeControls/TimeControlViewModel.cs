using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Enums;
using ChessWPF.Models.Options;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class TimeControlViewModel : ViewModelBase
    {
        private byte id;

        public TimeControlViewModel(byte id, ushort clockTime, ushort clockIncrement, TimeControlType timeControlType)
        {
            Id = id;
            TimeControl = new TimeControl(clockTime, clockIncrement, timeControlType);
            SelectTimeControlCommand = new SelectTimeControlCommand(this);
        }

        public byte Id
        {
            get => id;
            init => id = value;
        }
        public bool IsSelected { get; set; }

        public string TimeAsText { get => TimeControl.ToString()!; }
        public TimeControl TimeControl { get; init; }
        public ICommand SelectTimeControlCommand { get; }
        public delegate void SelectTimeControlEventHandler(object? sender, SelectTimeControlEventArgs eventArgs);
        public event SelectTimeControlEventHandler Select;

        public void SelectChanged()
        {
            this.IsSelected = true;
            Select?.Invoke(this, new SelectTimeControlEventArgs(this.Id));
        }
    }
}
