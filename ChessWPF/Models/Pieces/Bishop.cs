using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public sealed class Bishop : Piece
    {
        public Bishop(PieceColor color) : base(PieceType.Bishop, color)
        {
        }

        public Bishop(PieceColor color, int row, int col) : base(PieceType.Bishop, color, row, col)
        {
        }
    }
}
