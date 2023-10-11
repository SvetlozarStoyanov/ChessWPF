using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;

namespace ChessWPF.Models.Data.Pieces
{
    public sealed class King : Piece
    {
        public King(PieceColor color) : base(PieceType.King, color)
        {
            Attackers = new List<Piece>();
            Defenders = new List<(Piece, Piece)>();
        }

        public King(PieceColor color, Cell cell) : base(PieceType.King, color, cell)
        {
            Attackers = new List<Piece>();
            Defenders = new List<(Piece, Piece)>();

        }
        public bool IsInCheck { get; set; }
        public List<Piece> Attackers { get; set; }
        public List<ValueTuple<Piece, Piece>> Defenders { get; set; }
    }
}
