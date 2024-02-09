using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class ResetBoardToDefaultCommand : CommandBase
    {
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;

        public ResetBoardToDefaultCommand(BoardConstructorMenuViewModel boardConstructorMenuViewModel)
        {
            this.boardConstructorMenuViewModel = boardConstructorMenuViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorMenuViewModel.ResetBoard();
        }
    }
}
