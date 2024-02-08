using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public sealed class Pawn : Piece
    {
        public Pawn(PieceColor color) : base(PieceType.Pawn, color)
        {
        }

        public Pawn(PieceColor color, int row, int col) : base(PieceType.Pawn, color, row, col)
        {
        }
    }
}
