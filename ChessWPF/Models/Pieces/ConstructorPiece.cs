using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public class ConstructorPiece
    {
        private PieceType pieceType;
        private PieceColor pieceColor;

        public ConstructorPiece(PieceType pieceType, PieceColor pieceColor)
        {
            PieceType = pieceType;
            PieceColor = pieceColor;
        }

        public PieceType PieceType
        {
            get { return pieceType; }
            set { pieceType = value; }
        }

        public PieceColor PieceColor
        {
            get { return pieceColor; }
            set { pieceColor = value; }
        }

    }
}
