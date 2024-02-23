using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Commands
{
    public class SelectDeletePieceCommand : CommandBase
    {
        private readonly BoardConstructorViewModel boardConstructorViewModel;

        public SelectDeletePieceCommand(BoardConstructorViewModel boardConstructorViewModel)
        {
            this.boardConstructorViewModel = boardConstructorViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorViewModel.SelectDeletePiece();
        }
    }
}
