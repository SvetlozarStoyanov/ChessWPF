using ChessWPF.Models.Data.Pieces;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class UpdateCellEventArgs
    {
        public UpdateCellEventArgs(Piece? piece)
        {
            this.Piece = piece;
        }
        public Piece? Piece { get; }
    }
}
