using ChessWPF.Contracts.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public class ConstructorMenuPiece : IConstructorPiece
    {
        private PieceType pieceType;
        private PieceColor pieceColor;

        public ConstructorMenuPiece(PieceType pieceType, PieceColor color)
        {
            PieceType = pieceType;
            Color = color;
        }

        public PieceType PieceType
        {
            get { return pieceType; }
            init { pieceType = value; }
        }

        public PieceColor Color
        {
            get { return pieceColor; }
            init { pieceColor = value; }
        }
    }
}
