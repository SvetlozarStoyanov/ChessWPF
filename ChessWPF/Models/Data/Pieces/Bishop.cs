using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public sealed class Bishop : Piece
    {
        public Bishop(PieceColor color) : base(PieceType.Bishop, color)
        {
        }

        public Bishop(PieceColor color, Cell cell) : base(PieceType.Bishop, color, cell)
        {
        }
    }
}
