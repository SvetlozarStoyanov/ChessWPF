﻿using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;

namespace ChessWPF.Models.Data.Pieces
{
    public sealed class Knight : Piece
    {
        public Knight(PieceColor color) : base(PieceType.Knight, color)
        {
        }

        public Knight(PieceColor color, int row, int col) : base(PieceType.Knight, color, row, col)
        {
        }
    }
}
