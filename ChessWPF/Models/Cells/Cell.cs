﻿using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Pieces;
using System;

namespace ChessWPF.Models.Cells
{
    public sealed class Cell
    {
        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public Cell(int row, int col, Piece? piece) : this(row, col)
        {
            Piece = piece;
        }

        public delegate void UpdateEventHandler(object? sender, UpdateCellEventArgs args);
        public event UpdateEventHandler Update;

        public delegate void UpdateForPromotionEventHandler(object? sender, UpdateCellEventArgs args);
        public event UpdateForPromotionEventHandler UpdateForPromotion;

        public delegate void CheckEventHandler(object? sender, EventArgs args);
        public event CheckEventHandler Check;
        public event CheckEventHandler UnCheck;

        public delegate void UpdateMovedToEventHandler(object? sender, EventArgs args);
        public event UpdateMovedToEventHandler UpdateMovedTo;

        public int Row { get; set; }
        public int Col { get; set; }
        public Piece? Piece { get; set; }

        public override bool Equals(object? obj)
        {
            var otherCell = obj as Cell;
            return this.Row == otherCell!.Row && this.Col == otherCell.Col && this.Piece?.PieceType == otherCell.Piece?.PieceType;
        }

        public bool HasEqualRowAndCol(object? obj)
        {
            var otherCell = obj as Cell;
            return this.Row == otherCell!.Row && this.Col == otherCell.Col;
        }

        public bool IsEvenCell()
        {
            return (Row + Col) % 2 == 0;
        }

        public int RowDifference(Cell cell)
        {
            return Math.Abs(this.Col - cell.Col);
        }

        public int ColumnDifference(Cell cell)
        {
            return Math.Abs(this.Col - cell.Col);
        }

        public void UpdateCell(Piece? piece)
        {
            this.Piece = piece;
            Update?.Invoke(this, new UpdateCellEventArgs(piece));
        }

        public void UpdateCellForPromotion(Piece piece)
        {
            this.Piece = piece;
            UpdateForPromotion?.Invoke(this, new UpdateCellEventArgs(piece));
        }

        public void MarkAsChecked()
        {
            Check?.Invoke(this, EventArgs.Empty);
        }

        public void MarkAsUnchecked()
        {
            UnCheck?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateMarkAsMovedTo()
        {
            UpdateMovedTo?.Invoke(this, EventArgs.Empty);
        }
    }
}
