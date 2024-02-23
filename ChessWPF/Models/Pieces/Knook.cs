using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public sealed class Knook : Piece
    {
        public Knook(PieceColor color) : base(PieceType.Knook, color)
        {
        }

        public Knook(PieceColor color, int row, int col) : base(PieceType.Knook, color, row, col)
        {
        }
    }
}
