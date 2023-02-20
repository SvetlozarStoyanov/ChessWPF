using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Game
{
    public static class PieceConstructor
    {
        public static Piece ConstructPieceByType(Piece piece, Cell cell)
        {
            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    return new Pawn(piece.Color, cell);
                case PieceType.Knight:
                    return new Knight(piece.Color, cell);
                case PieceType.Bishop:
                    return new Bishop(piece.Color, cell);
                case PieceType.Rook:
                    return new Rook(piece.Color, cell);
                case PieceType.Queen:
                    return new Queen(piece.Color, cell);
                case PieceType.King:
                    return new King(piece.Color, cell);
            }
            return null;
        }
    }
}
