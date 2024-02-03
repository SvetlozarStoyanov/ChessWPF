using ChessWPF.Contracts.Pieces;
using ChessWPF.Models.Data.Pieces;
using System;

namespace ChessWPF.Models.Data.Board
{
    public sealed class ConstructorCell
    {
        private int row;
        private int col;
        private ConstructorBoardPiece? constructorBoardPiece;

        public ConstructorCell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public ConstructorCell(int row, int col, ConstructorBoardPiece? constructorPiece) : this(row, col)
        {
            ConstructorBoardPiece = constructorPiece;
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

        public ConstructorBoardPiece? ConstructorBoardPiece
        {
            get { return constructorBoardPiece; }
            private set
            {
                constructorBoardPiece = value;
            }
        }

        public delegate void UpdateConstructorCellEventHandler(object? sender, EventArgs e);
        public event UpdateConstructorCellEventHandler Update;

        public void UpdatePiece(IConstructorPiece? constructorPiece)
        {
            if (constructorPiece == null)
            {
                ConstructorBoardPiece = null;
            }
            else if (ConstructorBoardPiece == null && constructorPiece != null)
            {
                ConstructorBoardPiece = new ConstructorBoardPiece(this.Row, this.Col, constructorPiece.Color, constructorPiece.PieceType);
            }
            else if (ConstructorBoardPiece != null && constructorPiece != null)
            {
                ConstructorBoardPiece.PieceType = constructorPiece.PieceType;
                ConstructorBoardPiece.Color = constructorPiece.Color;
            }
            Update(this, EventArgs.Empty);
        }
    }
}
