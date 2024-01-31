using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.ViewModels.Enums;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ChessWPF.ViewModels.Pieces
{
    public class ConstructorPieceViewModel : ViewModelBase
    {
        private BitmapImage pieceImage;
        private ConstructorPiece constructorPiece;

        public ConstructorPieceViewModel(ConstructorPiece constructorPiece)
        {
            ConstructorPiece = constructorPiece;
            SelectPieceFromPieceMenuCommand = new SelectPieceFromPieceMenuCommand(this);
            SetImage();
        }

        public event SelectConstructorPieceEventHandler SelectConstructorPiece;
        public delegate void SelectConstructorPieceEventHandler(object? sender, SelectConstructorPieceEventArgs e);

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

        public ConstructorPiece ConstructorPiece
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
            SelectConstructorPiece(null, new SelectConstructorPieceEventArgs(ConstructorPiece));
        }
    }
}
