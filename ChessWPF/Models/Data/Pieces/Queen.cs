using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
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
