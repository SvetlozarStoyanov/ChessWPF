using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Game
{
    public static class PieceCreator
    {
        public static Piece CreatePieceByProperties(PieceType pieceType, PieceColor color, int row, int col)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return new Pawn(color, row, col);
                case PieceType.Knight:
                    return new Knight(color, row, col);
                case PieceType.Bishop:
                    return new Bishop(color, row, col);
                case PieceType.Rook:
                    return new Rook(color, row, col);
                case PieceType.Queen:
                    return new Queen(color, row, col);
                case PieceType.Knook:
                    return new Knook(color, row, col);
                default:
                    return new King(color, row, col);
            }
        }

        public static Piece CreatePieceByLetterAndCoordinates(char letter, int row, int col)
        {
            var color = char.IsUpper(letter) ? PieceColor.White : PieceColor.Black;
            if (color == PieceColor.White)
            {
                letter = (char)(letter + 32);
            }
            switch (letter)
            {
                case 'p':
                    return new Pawn(color, row, col);
                case 'n':
                    return new Knight(color, row, col);
                case 'b':
                    return new Bishop(color, row, col);
                case 'r':
                    return new Rook(color, row, col);
                case 'q':
                    return new Queen(color, row, col);
                case 'o':
                    return new Knook(color, row, col);
                default:
                    return new King(color, row, col);
            }
        }

        public static Piece CreatePieceForPromotion(PieceType pieceType, PieceColor Color)
        {
            switch (pieceType)
            {
                case PieceType.Knight:
                    return new Knight(Color);
                case PieceType.Bishop:
                    return new Bishop(Color);
                case PieceType.Rook:
                    return new Rook(Color);
                case PieceType.Knook:
                    return new Knook(Color);
                default:
                    return new Queen(Color);
            }
        }
    }
}
