using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Game
{
    public static class PieceConstructor
    {
        public static Piece ConstructPieceByType(PieceType pieceType,PieceColor color, Cell cell)
        {
            switch (pieceType)
            {
                case PieceType.Pawn:
                    return new Pawn(color, cell);
                case PieceType.Knight:
                    return new Knight(color, cell);
                case PieceType.Bishop:
                    return new Bishop(color, cell);
                case PieceType.Rook:
                    return new Rook(color, cell);
                case PieceType.Queen:
                    return new Queen(color, cell);
                case PieceType.King:
                    return new King(color, cell);
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
                default:
                    return new Queen(Color);
            }
        }
    }
}
