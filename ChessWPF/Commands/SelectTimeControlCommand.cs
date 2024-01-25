using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class SelectTimeControlCommand : CommandBase
    {
        private readonly TimeControlViewModel timeControlViewModel;
        public SelectTimeControlCommand(TimeControlViewModel timeControlViewModel)
        {
            this.timeControlViewModel = timeControlViewModel;
        }

        public override void Execute(object? parameter)
        {
            timeControlViewModel.SelectChanged();
        }
    }
}
