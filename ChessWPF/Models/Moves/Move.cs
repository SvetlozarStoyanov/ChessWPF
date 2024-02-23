using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces.Enums;

namespace ChessWPF.Models.Moves
{
    public sealed class Move
    {
        public int CurrHalfMoveCount { get; set; }
        public string FenAnnotation { get; set; } = null!;
        public string Annotation { get; set; } = null!;
        public bool IsPromotionMove { get; set; } = false;
        public Cell CellOneBefore { get; set; } = null!;
        public Cell CellOneAfter { get; set; } = null!;
        public Cell CellTwoBefore { get; set; } = null!;
        public Cell CellTwoAfter { get; set; } = null!;
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

        public bool IsHalfMove()
        {
            if (this.CellOneBefore.Piece!.PieceType == PieceType.Pawn ||
                (this.CellTwoAfter.Piece != null && this.CellTwoBefore.Piece != null 
                && this.CellTwoBefore.Piece.Color != this.CellOneBefore.Piece.Color))
            {
                return false;
            }
            return true;
        }
    }
}
