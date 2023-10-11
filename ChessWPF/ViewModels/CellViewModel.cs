using ChessWPF.Commands;
using ChessWPF.Models.Data.Board;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.ViewModels
{
    public sealed class CellViewModel : ViewModelBase
    {
        private Cell cell;

        private BitmapImage cellImage;
        private bool canBeMovedTo;
        private bool isSelected;
        private bool canBeSelected;
        private bool canBeSelectedForPromotion;
        private bool isInCheck;




        public CellViewModel(Cell cell)
        {
            this.cell = cell;
            SelectCommand = new SelectCommand(this);
            MoveCommand = new MoveCommand(this);
            PromoteCommand = new PromoteCommand(this);
            CheckCommand = new CheckCommand(this);
            //CanBeMovedTo = false;
            //IsSelected = false;
            //CanBeSelectedForPromotion = false;
        }

        public Cell Cell
        {
            get
            {
                return cell;
            }
            set
            {
                cell = value;
                OnPropertyChanged(nameof(Cell));
            }
        }

        public BitmapImage CellImage
        {
            get
            {
                return cellImage;
            }
            set
            {
                cellImage = value;
                OnPropertyChanged(nameof(CellImage));
            }
        }

        public bool CanBeMovedTo
        {
            get { return canBeMovedTo; }
            set
            {
                canBeMovedTo = value;
                OnPropertyChanged(nameof(CanBeMovedTo));
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool CanBeSelected
        {
            get { return canBeSelected; }
            set
            {
                canBeSelected = value;
                OnPropertyChanged(nameof(CanBeSelected));
            }
        }

        public bool CanBeSelectedForPromotion
        {
            get { return canBeSelectedForPromotion; }
            set
            {
                canBeSelectedForPromotion = value;
                OnPropertyChanged(nameof(CanBeSelectedForPromotion));
            }
        }

        public bool IsInCheck
        {
            get { return isInCheck; }
            set
            {
                isInCheck = value;
                OnPropertyChanged(nameof(IsInCheck));
            }
        }

        public ICommand SelectCommand { get; set; }
        public ICommand MoveCommand { get; set; }
        public ICommand PromoteCommand { get; set; }
        public ICommand CheckCommand { get; set; }

        public void UpdateCellImage()
        {
            if (cell.Piece != null)
            {
                string imageUrl = $"/Graphics/Chess Pieces/{cell.Piece.Color} {cell.Piece.GetType().Name}.png";
                Uri resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
                CellImage = new BitmapImage(resourceUri);
            }
            else
            {
                CellImage = null;
            }
        }

        //public void UpdateCanBeSelected()
        //{
        //    if (cell.Piece != null)
        //        CanBeSelected = true;
        //    else
        //        CanBeSelected = false;
        //}
    }
}
