using ChessWPF.Models.Data.Pieces;
using System;

namespace ChessWPF.Models.Data.Board
{
    public sealed class ConstructorCell
    {
        private int row;
        private int col;
        private ConstructorPiece? constructorPiece;

        public ConstructorCell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public ConstructorCell(int row, int col, ConstructorPiece? constructorPiece) : this(row, col)
        {
            ConstructorPiece = constructorPiece;
        }

        public int Row
        {
            get { return row; }
            init { row = value; }
        }

        public int Col
        {
            get { return col; }
            init { col = value; }
        }

        public ConstructorPiece? ConstructorPiece
        {
            get { return constructorPiece; }
            private set
            {
                constructorPiece = value;
            }
        }

        public delegate void UpdateConstructorCellEventHandler(object? sender, EventArgs e);
        public event UpdateConstructorCellEventHandler Update;

        public void UpdatePiece(ConstructorPiece constructorPiece)
        {
            ConstructorPiece = constructorPiece;
            Update(this, EventArgs.Empty);
        }
    }
}
