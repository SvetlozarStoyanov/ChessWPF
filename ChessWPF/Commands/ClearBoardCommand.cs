using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class ClearBoardCommand : CommandBase
    {
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;

        public ClearBoardCommand(BoardConstructorMenuViewModel boardConstructorMenuViewModel)
        {
            this.boardConstructorMenuViewModel = boardConstructorMenuViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorMenuViewModel.ClearPieces();
        }
    }
}
