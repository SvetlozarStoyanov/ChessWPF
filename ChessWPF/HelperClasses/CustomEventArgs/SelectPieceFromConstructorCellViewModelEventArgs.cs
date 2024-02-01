using ChessWPF.Models.Data.Pieces;
using System;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class SelectPieceFromConstructorCellViewModelEventArgs : EventArgs
    {
        public SelectPieceFromConstructorCellViewModelEventArgs(int row, int col, ConstructorBoardPiece constructorPiece)
        {
            Row = row;
            Col = col;
            ConstructorPiece = constructorPiece;
        }

        public int Row { get; init; }
        public int Col { get; init; }
        public ConstructorBoardPiece ConstructorPiece { get; init; }
    }
}
