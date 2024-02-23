using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public sealed class Rook : Piece
    {
        public Rook(PieceColor color) : base(PieceType.Rook, color)
        {
        }

        public Rook(PieceColor color, int row, int col) : base(PieceType.Rook, color, row, col)
        {
        }
    }
}
