using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
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
