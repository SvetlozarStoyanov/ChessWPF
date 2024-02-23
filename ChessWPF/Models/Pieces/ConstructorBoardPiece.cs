using ChessWPF.Contracts.Pieces;
using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Pieces
{
    public class ConstructorBoardPiece : IConstructorPiece
    {
        private PieceType pieceType;
        private PieceColor pieceColor;

        public ConstructorBoardPiece(int row, int col, PieceColor pieceColor, PieceType pieceType)
        {
            Row = row;
            Col = col;
            PieceType = pieceType;
            Color = pieceColor;
        }
        public int Row { get; set; }
        public int Col { get; set; }

        public PieceType PieceType
        {
            get { return pieceType; }
            set { pieceType = value; }
        }

        public PieceColor Color
        {
            get { return pieceColor; }
            set { pieceColor = value; }
        }

    }
}
