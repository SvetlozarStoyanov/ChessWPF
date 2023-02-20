using ChessWPF.Models.Data.Pieces;
using System.Collections.Generic;
using System.Windows.Documents;

namespace ChessWPF.Models.Data.Board
{
    public class Cell
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
            return this.Row == (obj as Cell).Row && this.Col == (obj as Cell).Col;
        }
    }
}
