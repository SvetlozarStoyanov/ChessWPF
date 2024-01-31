using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Commands
{
    public class SelectPieceSelectorCommand : CommandBase
    {
        private readonly BoardConstructorViewModel boardConstructorViewModel;

        public SelectPieceSelectorCommand(BoardConstructorViewModel boardConstructorViewModel)
        {
            this.boardConstructorViewModel = boardConstructorViewModel;
        }

        public override void Execute(object? parameter)
        {
            boardConstructorViewModel.EnableSelectingPiecesFromBoard();
        }
    }
}
