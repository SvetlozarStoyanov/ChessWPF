using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Commands
{
    public class SetCastlingRightsCommand : CommandBase
    {
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;

        public SetCastlingRightsCommand(BoardConstructorMenuViewModel boardConstructorMenuViewModel)
        {
            this.boardConstructorMenuViewModel = boardConstructorMenuViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorMenuViewModel.UpdateCastlingRights();
        }
    }
}
