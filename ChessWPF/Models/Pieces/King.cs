﻿using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;

namespace ChessWPF.Models.Pieces
{
    public sealed class King : Piece
    {
        public King(PieceColor color) : base(PieceType.King, color)
        {
            Attackers = new List<Piece>();
            Defenders = new List<(Piece, Piece)>();
        }

        public King(PieceColor color, int row, int col) : base(PieceType.King, color, row, col)
        {
            Attackers = new List<Piece>();
            Defenders = new List<(Piece, Piece)>();

        }
        public bool IsInCheck { get; set; }
        public List<Piece> Attackers { get; set; }
        public List<ValueTuple<Piece, Piece>> Defenders { get; set; }
    }
}
