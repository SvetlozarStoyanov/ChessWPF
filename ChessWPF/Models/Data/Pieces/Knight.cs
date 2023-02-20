using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public class Knight : Piece
    {
        public Knight(PieceColor color) : base(PieceType.Knight, color)
        {
        }

        public Knight(PieceColor color, Cell cell) : base(PieceType.Knight, color, cell)
        {
        }
    }
}
