using ChessWPF.Models.Data.Pieces;

namespace ChessWPF.Models.Data.Board
{
    public class Cell
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Piece? Piece { get; set; }
    }
}
