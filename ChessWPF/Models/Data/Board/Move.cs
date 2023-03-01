using System.Reflection.Emit;

namespace ChessWPF.Models.Data.Board
{
    public class Move
    {
        public Cell CellOneBefore { get; set; }
        public Cell CellOneAfter { get; set; }
        public Cell CellTwoBefore { get; set; }
        public Cell CellTwoAfter { get; set; }

        public Cell? CellThreeBefore { get; set; }
        public Cell? CellThreeAfter { get; set; }
        public Cell? CellFourBefore { get; set; }
        public Cell? CellFourAfter { get; set; }

        public override bool Equals(object? obj)
        {
            var otherMove = obj as Move;
            if (this.CellOneBefore.Equals(otherMove.CellOneBefore) 
                && this.CellTwoBefore.Equals(otherMove.CellTwoBefore)
                && this.CellOneAfter.Equals(otherMove.CellOneAfter)
                && this.CellTwoAfter.Equals(otherMove.CellTwoAfter))
            {
                return true;
            }
            return false;
        }

        public bool IsOppositeMove(Move otherMove)
        {
            if (this.CellOneBefore.Equals(otherMove.CellTwoAfter)
            && this.CellTwoBefore.Equals(otherMove.CellOneAfter)
            && this.CellOneAfter.Equals(otherMove.CellTwoBefore)
            && this.CellTwoAfter.Equals(otherMove.CellOneBefore))
            {
                return true;
            }
            return false;
        }
    }
}
