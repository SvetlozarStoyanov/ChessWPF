using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public class Queen : Piece
    {
        public Queen(PieceColor color) : base(PieceType.Queen, color)
        {
        }

        public Queen(PieceColor color, Cell cell) : base(PieceType.Queen, color, cell)
        {
        }
    }
}
