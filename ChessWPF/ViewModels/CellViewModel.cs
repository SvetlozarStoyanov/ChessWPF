using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Board;
using ChessWPF.ViewModels.Enums;
using System;
using System.Collections.Generic;
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
        private bool isOccupied;
        private string selectorImage;
        private Color movedToColor;
        private Color defaultColor;
        private Cell cell = null!;
        private SolidColorBrush backgroundBrush = new SolidColorBrush();
        private BitmapImage? cellPieceImage;
        private Dictionary<SelectorStates, string> selectors;

        public CellViewModel(Cell cell, Color defaultColor, Color movedToColor)
        {
            Cell = cell;
            Cell.Update += Update;
            Cell.UpdateForPromotion += UpdateForPromotion;
            Cell.Check += OnCheck;
            Cell.UnCheck += OnUnCheck;
            Cell.UpdateMovedTo += UpdateMarkAsMovedTo;
            this.defaultColor = defaultColor;
            this.movedToColor = movedToColor;
            BackgroundBrush = new SolidColorBrush(defaultColor);

            SelectCommand = new SelectCommand(this);
            MoveCommand = new MoveCommand(this);
            PromoteCommand = new PromoteCommand(this);
            IsOccupied = false;
            

            selectors = new Dictionary<SelectorStates, string>()
            {
                [SelectorStates.Empty] = "/Graphics/Selectors/Empty Selector.png",
                [SelectorStates.Occupied] = "/Graphics/Selectors/Occupied Selector.png"
            };

            //var emptyCellUrl = "/Graphics/Empty Cell.png";
            CellPieceImage = null;
            SelectorImage = @$"pack://application:,,,{selectors[SelectorStates.Empty]}";
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
        public bool IsOccupied
        {
            get { return isOccupied; }
            set 
            { 
                isOccupied = value;
                OnPropertyChanged(nameof(IsOccupied));
            }
        }

        public Cell Cell
        {
            get => cell;
            set
            {
                cell = value;
                OnPropertyChanged(nameof(Cell));
            }
        }

        public BitmapImage? CellPieceImage
        {
            get => cellPieceImage;
            set
            {
                cellPieceImage = value;
                OnPropertyChanged(nameof(CellPieceImage));
            }
        }

        public string SelectorImage
        {
            get { return selectorImage; }
            set
            {
                selectorImage = value;
                OnPropertyChanged(nameof(SelectorImage));
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
                var pieceImageUri = new Uri(@$"pack://application:,,,{imageUrl}");
                CellPieceImage = new BitmapImage(pieceImageUri);
                SelectorImage = @$"pack://application:,,,{selectors[SelectorStates.Occupied]}";
                IsOccupied = true;
            }
            else
            {
                if (!this.CanBeSelectedForPromotion)
                {
                    //var emptyCellUrl = $"/Graphics/Empty Cell.png";
                    CellPieceImage = null;
                    SelectorImage = @$"pack://application:,,,{selectors[SelectorStates.Empty]}";
                    IsOccupied = false;
                }
            }
        }

        public void UpdateMarkAsMovedTo(object? sender, EventArgs args)
        {
            IsPartOfLastMove = !IsPartOfLastMove;
            if (IsPartOfLastMove)
            {
                BackgroundBrush.Color = movedToColor;
            }
            else
            {
                BackgroundBrush.Color = defaultColor;
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
