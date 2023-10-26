using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
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
