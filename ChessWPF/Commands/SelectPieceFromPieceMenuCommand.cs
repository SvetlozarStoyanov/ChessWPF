using ChessWPF.ViewModels.Pieces;
using System;

namespace ChessWPF.Commands
{
    public class SelectPieceFromPieceMenuCommand : CommandBase
    {
        private readonly ConstructorMenuPieceViewModel constructorPieceViewModel;

        public SelectPieceFromPieceMenuCommand(ConstructorMenuPieceViewModel constructorPieceViewModel)
        {
            this.constructorPieceViewModel = constructorPieceViewModel;
        }

        public override void Execute(object? parameter)
        {
            constructorPieceViewModel.Select();
        }
    }
}
