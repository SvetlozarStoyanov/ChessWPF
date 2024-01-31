using ChessWPF.ViewModels.Pieces;
using System;

namespace ChessWPF.Commands
{
    public class SelectPieceFromPieceMenuCommand : CommandBase
    {
        private readonly ConstructorPieceViewModel constructorPieceViewModel;

        public SelectPieceFromPieceMenuCommand(ConstructorPieceViewModel constructorPieceViewModel)
        {
            this.constructorPieceViewModel = constructorPieceViewModel;
        }

        public override void Execute(object? parameter)
        {
            constructorPieceViewModel.Select();
        }
    }
}
