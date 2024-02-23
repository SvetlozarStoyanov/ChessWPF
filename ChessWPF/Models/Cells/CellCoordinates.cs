using System;

namespace ChessWPF.Models.Cells
{
    public struct CellCoordinates
    {
        public CellCoordinates(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public int Row { get; init; }
        public int Col { get; init; }
        public string AsText
        {
            get => this.ToString();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is CellCoordinates))
            {
                return false;
            }
            var other = (obj as CellCoordinates?)!.Value;

            return this.Row == other.Row && this.Col == other.Col;
        }

        public override string ToString()
        {
            if (Row == -1 && Col == -1)
            {
                return "";
            }
            return $"{(char)(Col + 97)}{7 - Row + 1}";
        }

        public int RowDifference(CellCoordinates other)
        {
            return Math.Abs(this.Row - other.Row);
        }
         
        public int ColDifference(CellCoordinates other)
        {
            return Math.Abs(this.Col - other.Col);
        }

        public bool HasEqualCoordinates(int row, int col)
        {
            return this.Row == row && this.Col == col;
        } 

        public bool HasEqualCoordinates(CellCoordinates other)
        {
            return this.Row == other.Row && this.Col == other.Col;
        }
    }
}
