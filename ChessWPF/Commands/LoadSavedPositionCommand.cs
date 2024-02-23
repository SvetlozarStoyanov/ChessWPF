using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class LoadSavedPositionCommand : CommandBase
    {
        private BoardConstructorMenuViewModel boardConstructorMenuViewModel;

        public LoadSavedPositionCommand(BoardConstructorMenuViewModel boardConstructorMenuViewModel)
        {
            this.boardConstructorMenuViewModel = boardConstructorMenuViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorMenuViewModel.LoadSavedPosition();
        }
    }
}
