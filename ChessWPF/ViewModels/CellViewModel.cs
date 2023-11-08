using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.ViewModels
{
    public sealed class CellViewModel : ViewModelBase
    {

        private bool canBeMovedTo;
        private bool isSelected;
        private bool canBeSelected;
        private bool canBeSelectedForPromotion;
        private bool isInCheck;
        private bool isPartOfLastMove;
        private Cell cell = null!;
        private BitmapImage? cellImage;
        private SolidColorBrush backgroundBrush = new SolidColorBrush();
        private Color defaultColor;
        private Color movedToColor;

        public CellViewModel(Cell cell)
        {
            Cell = cell;
            Cell.Update += Update;
            Cell.UpdateForPromotion += UpdateForPromotion;
            Cell.Check += OnCheck;
            Cell.UnCheck += OnUnCheck;
            SelectCommand = new SelectCommand(this);
            MoveCommand = new MoveCommand(this);
            PromoteCommand = new PromoteCommand(this);
        }

        public delegate void SelectEventHandler(object sender, SelectCellViewModelEventArgs args);
        public event SelectEventHandler Select;

        public delegate void MovedToEventHandler(object sender, MovedToCellViewModelEventArgs args);
        public event MovedToEventHandler MovedTo;

        public delegate void PromotedToEventHandler(object sender, PromotePieceEventArgs args);
        public event PromotedToEventHandler PromotedTo;

        

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
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public bool CanBeSelected
        {
            get => canBeSelected;
            set
            {
                canBeSelected = value;
                OnPropertyChanged(nameof(CanBeSelected));
            }
        }

        public bool CanBeSelectedForPromotion
        {
            get => canBeSelectedForPromotion;
            set
            {
                canBeSelectedForPromotion = value;
                OnPropertyChanged(nameof(CanBeSelectedForPromotion));
            }
        }

        public bool IsInCheck
        {
            get => isInCheck;
            set
            {
                isInCheck = value;
                OnPropertyChanged(nameof(IsInCheck));
            }
        }
        public bool IsPartOfLastMove
        {
            get => isPartOfLastMove;
            set
            {
                isPartOfLastMove = value;
                OnPropertyChanged(nameof(IsPartOfLastMove));
            }
        }

        public BitmapImage? CellImage
        {
            get => cellImage;
            set
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

        public ICommand SelectCommand { get; set; }
        public ICommand MoveCommand { get; set; }
        public ICommand PromoteCommand { get; set; }

        public void SelectCell()
        {
            OnSelect();
        }

        public void MoveToCell()
        {
            OnMovedTo();
        }

        public void OnPromotedTo()
        {
            PromotedTo(this, new PromotePieceEventArgs(this.Cell.Piece!.PieceType));
        }

        private void OnMovedTo()
        {
            this.CanBeMovedTo = false;
            MovedTo(this, new MovedToCellViewModelEventArgs(this.Cell));
        }

        public void UpdateCellImage()
        {
            if (cell.Piece != null)
            {
                var imageUrl = $"/Graphics/Chess Pieces/{cell.Piece.Color} {cell.Piece.GetType().Name}.png";
                var resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
                CellImage = new BitmapImage(resourceUri);
            }
            else
            {
                CellImage = null;
            }
        }

        private void OnSelect()
        {
            if (!this.IsSelected)
            {
                IsSelected = true;
            }
            else
            {
                IsSelected = false;
            }
            Select(this, new SelectCellViewModelEventArgs(this));
        }

        private void Update(object? sender, UpdateCellEventArgs args)
        {
            UpdateCellImage();
            CanBeSelected = false;
        }

        private void UpdateForPromotion(object? sender, UpdateCellEventArgs args)
        {
            UpdateCellImage();
            CanBeSelectedForPromotion = true;
            IsInCheck = false;
            CanBeSelected = false;
        }

        private void OnCheck(object? sender, EventArgs args)
        {
            this.IsInCheck = true;
        }

        private void OnUnCheck(object? sender, EventArgs args)
        {
            this.IsInCheck = false;
        }
    }
}
