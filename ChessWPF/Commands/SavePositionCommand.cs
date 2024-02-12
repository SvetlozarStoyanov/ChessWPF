using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class SavePositionCommand : CommandBase
    {
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;

        public SavePositionCommand(BoardConstructorMenuViewModel boardConstructorMenuViewModel)
        {
            this.boardConstructorMenuViewModel = boardConstructorMenuViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorMenuViewModel.SavePosition();
        }
    }
}
