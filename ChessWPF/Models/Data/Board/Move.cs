namespace ChessWPF.Models.Data.Board
{
    public class Move
    {
        public int Number { get; set; }
        public Cell CellOneBefore { get; set; }
        public Cell CellOneAfter { get; set; }
        public Cell CellTwoBefore { get; set; }
        public Cell CellTwoAfter { get; set; }

        public Cell? CellThreeBefore { get; set; }
        public Cell? CellThreeAfter { get; set; }
        public Cell? CellFourBefore { get; set; }
        public Cell? CellFourAfter { get; set; }

        public bool IsCastling { get; set; }
    }
}
