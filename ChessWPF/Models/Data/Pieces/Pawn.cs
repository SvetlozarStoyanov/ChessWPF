using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public sealed class Pawn : Piece
    {
        public Pawn(PieceColor color) : base(PieceType.Pawn, color)
        {
        }

        public Pawn(PieceColor color, Cell cell) : base(PieceType.Pawn, color, cell)
        {
        }
    }
}
