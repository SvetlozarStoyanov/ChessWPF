using ChessWPF.Models.Data.Pieces;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class SelectConstructorPieceEventArgs
    {
        public SelectConstructorPieceEventArgs(ConstructorPiece? constructorPiece)
        {
            ConstructorPiece = constructorPiece;
        }

        public ConstructorPiece? ConstructorPiece { get; init; }

    }
}
