using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public sealed class Queen : Piece
    {
        public Queen(PieceColor color) : base(PieceType.Queen, color)
        {
        }

        public Queen(PieceColor color, int row, int col) : base(PieceType.Queen, color, row, col)
        {
        }
    }
}
