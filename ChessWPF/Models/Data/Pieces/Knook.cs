using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public sealed class Knook : Piece
    {
        public Knook(PieceColor color) : base(PieceType.Knook, color)
        {
        }

        public Knook(PieceColor color, Cell cell) : base(PieceType.Knook, color, cell)
        {
        }
    }
}
