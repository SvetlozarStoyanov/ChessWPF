using ChessWPF.Models.Pieces;

namespace ChessWPF.HelperClasses.CustomEventArgs
{
    public class SelectMenuPieceEventArgs
    {
        public SelectMenuPieceEventArgs(ConstructorMenuPiece? menuPiece)
        {
            MenuPiece = menuPiece;
        }

        public ConstructorMenuPiece? MenuPiece { get; init; }

    }
}
