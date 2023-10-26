using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Game
{
    public static class PieceConstructor
    {
        public static Piece ConstructPieceByType(PieceType pieceType, PieceColor color, int row, int col)
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
                case PieceType.King:
                    return new King(color, row, col);
            }
            return null;
        }

        public static Piece ConstructPieceForPromotion(PieceType pieceType, PieceColor Color)
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
