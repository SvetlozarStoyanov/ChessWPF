using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Contracts.Pieces
{
    public interface IConstructorPiece
    {
        public PieceColor Color { get;  }
        public PieceType PieceType { get;  }
    }
}
