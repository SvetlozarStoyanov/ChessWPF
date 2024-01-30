using ChessWPF.Models.Data.Board;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.ViewModels
{
    public class ConstructorCellViewModel : ViewModelBase
    {
        private BitmapImage? cellImage;
        private SolidColorBrush backgroundBrush;
        private ConstructorCell constructorCell;

        public ConstructorCellViewModel(ConstructorCell constructorCell, Color color)
        {
            ConstructorCell = constructorCell;
            ConstructorCell.Update += UpdateCellImage;
            BackgroundBrush = new SolidColorBrush(color);
        }

        public BitmapImage? CellImage
        {
            get { return cellImage; }
            private set
            { 
                cellImage = value;
                OnPropertyChanged(nameof(CellImage));
            }
        }


        public SolidColorBrush BackgroundBrush
        {
            get => backgroundBrush;
            set
            {
                backgroundBrush = value;
                OnPropertyChanged(nameof(BackgroundBrush.Color));
            }
        }

        public ConstructorCell ConstructorCell
        {
            get { return constructorCell; }
            set { constructorCell = value; }
        }

        public void UpdateCellImage(object? sender, EventArgs e)
        {
            if (constructorCell.ConstructorPiece != null )
            {
                var imageUrl = $"/Graphics/Chess Pieces/{constructorCell.ConstructorPiece.PieceColor} {constructorCell.ConstructorPiece.PieceType}.png";
                var pieceImageUri = new Uri(@$"pack://application:,,,{imageUrl}");
                CellImage = new BitmapImage(pieceImageUri);
            }
            else
            {
                CellImage = null;
            }
        }
    }
}
