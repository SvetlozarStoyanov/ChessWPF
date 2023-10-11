using ChessWPF.Models.Data.Pieces;
using System;

namespace ChessWPF.Models.Data.Board
{
    public sealed class Cell
    {
        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public Cell(int row, int col, Piece? piece) : this(row, col)
        {
            Piece = piece;

        }
        public int Row { get; set; }
        public int Col { get; set; }

        public Piece? Piece { get; set; }

        public override bool Equals(object? obj)
        {
            var otherCell = obj as Cell;
            return this.Row == otherCell.Row && this.Col == otherCell.Col && this.Piece?.PieceType == otherCell.Piece?.PieceType;
        }

        public bool HasEqualRowAndCol(object? obj)
        {
            var otherCell = obj as Cell;
            return this.Row == otherCell.Row && this.Col == otherCell.Col;
        }

        public bool IsEvenCell()
        {
            if ((Row + Col) % 2 == 0)
            {
                return true;
            }
            return false;
        }

        public int RowDifference(Cell cell)
        {
            return Math.Abs(this.Col - cell.Col);
        }

        public int ColumnDifference(Cell cell)
        {
            return Math.Abs(this.Col - cell.Col);
        }
    }
}
