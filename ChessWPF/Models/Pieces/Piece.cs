﻿using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces.Enums;
using System.Collections.Generic;

namespace ChessWPF.Models.Pieces
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

        public Piece(PieceType pieceType, PieceColor color, int row, int col) : this(pieceType, color)
        {
            Row = row;
            Col = col;
        }

        public PieceType PieceType { get; set; }
        public PieceColor Color { get; set; }
        public int Row { get; private set; }
        public int Col { get; private set; }

        public List<Cell> LegalMoves { get; set; }
        public List<Cell> ProtectedCells { get; set; }

        public override bool Equals(object? obj)
        {
            var otherPiece = (obj as Piece)!;
            return otherPiece.Row == this.Row
                && otherPiece.Col == this.Col
                && otherPiece.PieceType == this.PieceType
                && otherPiece.Color == this.Color;
        }

        public bool HasEqualCoordinatesWithCell(object? obj)
        {
            var cell = obj as Cell;
            return cell!.Row == this.Row && cell.Col == this.Col;
        }

        public bool HasEqualCoordinates(int row, int col)
        {
            return this.Row == row && this.Col == col;
        }

        public bool HasEvenCoordinates()
        {
            return (Row + Col) % 2 == 0;
        }

        public void SetCoordinates(Cell cell)
        {
            this.Row = cell.Row;
            this.Col = cell.Col;
        }

        public void UpdateLegalMovesAndProtectedCells(List<Cell> legalMoves, List<Cell> protectedCells)
        {
            this.LegalMoves = legalMoves;
            this.ProtectedCells = protectedCells;
        }


    }
}