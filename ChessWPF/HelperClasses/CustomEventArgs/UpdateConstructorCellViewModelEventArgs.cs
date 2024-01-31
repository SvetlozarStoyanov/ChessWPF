using ChessWPF.Models.Data.Pieces;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class UpdateConstructorCellViewModelEventArgs
    {
        public UpdateConstructorCellViewModelEventArgs(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public int Row { get; init; }
        public int Col { get; init; }
    }
}
