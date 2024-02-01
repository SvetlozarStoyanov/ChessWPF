using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Pieces;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessWPF.ViewModels.Pieces
{
    public class ConstructorMenuPieceViewModel : ViewModelBase
    {
        private BitmapImage pieceImage;
        private ConstructorMenuPiece constructorPiece;

        public ConstructorMenuPieceViewModel(ConstructorMenuPiece constructorPiece)
        {
            ConstructorPiece = constructorPiece;
            SelectPieceFromPieceMenuCommand = new SelectPieceFromPieceMenuCommand(this);
            SetImage();
        }

        public event SelectConstructorPieceEventHandler SelectConstructorPiece;
        public delegate void SelectConstructorPieceEventHandler(object? sender, SelectMenuPieceEventArgs e);

        public ICommand SelectPieceFromPieceMenuCommand { get; init; }

        public BitmapImage PieceImage
        {
            get { return pieceImage; }
            private set
            {
                pieceImage = value;
                OnPropertyChanged(nameof(PieceImage));
            }
        }

        public ConstructorMenuPiece ConstructorPiece
        {
            get => constructorPiece;
            private set => constructorPiece = value;
        }

        public void SetImage()
        {
            var imageUrl = $"/Graphics/Chess Pieces/{ConstructorPiece.Color} {ConstructorPiece.PieceType}.png";
            var pieceImageUri = new Uri(@$"pack://application:,,,{imageUrl}");
            PieceImage = new BitmapImage(pieceImageUri);
        }

        public void Select()
        {
            SelectConstructorPiece(null, new SelectMenuPieceEventArgs(ConstructorPiece));
        }
    }
}
