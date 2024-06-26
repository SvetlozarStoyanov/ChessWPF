﻿using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Cells;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.ViewModels
{
    public sealed class ConstructorCellViewModel : ViewModelBase
    {
        private bool isSelected;
        private bool canBeSelected;
        private Color selectedColor;
        private Color defaultColor;
        private BitmapImage? cellImage;
        private SolidColorBrush backgroundBrush;
        private ConstructorCell constructorCell;

        public ConstructorCellViewModel(ConstructorCell constructorCell, Color defaultColor, Color selectedColor)
        {
            ConstructorCell = constructorCell;
            ConstructorCell.Update += UpdateCellImage;
            this.defaultColor = defaultColor;
            this.selectedColor = selectedColor;
            BackgroundBrush = new SolidColorBrush(defaultColor);
            UpdateConstructorCellCommand = new UpdateConstructorCellCommand(this);
            SelectPieceFromCellCommand = new SelectPieceFromCellCommand(this);
        }

        public event UpdateConstructorCellViewModelEventHandler UpdateConstructorCellViewModel;
        public delegate void UpdateConstructorCellViewModelEventHandler(object? sender, UpdateConstructorCellViewModelEventArgs e);
        public event SelectPieceFromConstructorCellViewModelCellEventHandler SelectPieceFromConstructorCellViewModelCell;
        public delegate void SelectPieceFromConstructorCellViewModelCellEventHandler(object? sender, SelectPieceFromConstructorCellViewModelEventArgs e);


        public bool CanBeSelected
        {
            get { return canBeSelected; }
            private set
            {
                canBeSelected = value;
                OnPropertyChanged(nameof(CanBeSelected));
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            private set
            {
                isSelected = value;
                if (!isSelected)
                {
                    BackgroundBrush.Color = defaultColor;
                }
                else
                {
                    BackgroundBrush.Color = selectedColor;
                }
                OnPropertyChanged(nameof(BackgroundBrush));
            }
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

        public ICommand UpdateConstructorCellCommand { get; init; }

        public ICommand SelectPieceFromCellCommand { get; init; }

        public void UpdateCellImage(object? sender, EventArgs e)
        {
            if (constructorCell.ConstructorBoardPiece != null)
            {
                var imageUrl = $"/Graphics/Chess Pieces/{ConstructorCell.ConstructorBoardPiece.Color} {ConstructorCell.ConstructorBoardPiece.PieceType}.png";
                var pieceImageUri = new Uri(@$"pack://application:,,,{imageUrl}");
                CellImage = new BitmapImage(pieceImageUri);
            }
            else
            {
                CellImage = null;
            }
            if (IsSelected)
            {
                UnSelect();
            }
        }

        public void UnSelect()
        {
            IsSelected = false;
        }

        public void Select()
        {
            if (IsSelected)
            {
                UnSelect();
            }
            else
            {
                IsSelected = true;
                SelectPiece();
            }
        }

        public void UpdateCellPiece()
        {
            UpdateConstructorCellViewModel(null, new UpdateConstructorCellViewModelEventArgs(ConstructorCell.Row, ConstructorCell.Col));
        }

        private void SelectPiece()
        {
            SelectPieceFromConstructorCellViewModelCell(null, new SelectPieceFromConstructorCellViewModelEventArgs(ConstructorCell.Row,
                ConstructorCell.Col,
                ConstructorCell.ConstructorBoardPiece!));
        }


        public void UpdateCanBeSelected(Type type, bool canBeSelected)
        {
            if (type.Equals(typeof(BoardConstructorViewModel)))
            {
                CanBeSelected = canBeSelected;
            }
            else
            {
                throw new InvalidOperationException($"Can only be called from {nameof(BoardConstructorViewModel)}");
            }
        }

    }
}
