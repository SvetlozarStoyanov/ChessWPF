using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;

namespace ChessWPF.Models.Data.Pieces
{
    public abstract class Piece
    {
        public Piece(PieceType pieceType, PieceColor color)
        {
            PieceType = pieceType;
            Color = color;
            LegalMoves = new List<Cell>();
            ProtectedCells = new List<Cell>();
        }
        public Piece(PieceType pieceType, PieceColor color, Cell cell) : this(pieceType, color)
        {
            Cell = cell;
        }
        public PieceType PieceType { get; set; }
        public PieceColor Color { get; set; }
        public Cell Cell { get; set; }

        public List<Cell> LegalMoves { get; set; }
        public List<Cell> ProtectedCells { get; set; }
    }
}