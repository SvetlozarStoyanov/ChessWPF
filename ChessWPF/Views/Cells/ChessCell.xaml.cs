﻿using ChessWPF.HelperClasses.WindowDimensions;
using ChessWPF.ViewModels;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.Views.Cells
{
    /// <summary>
    /// Interaction logic for ChessCell.xaml
    /// </summary>
    public partial class ChessCell : UserControl
    {
        private ICommand selectCommand;
        private Regex pieceRegex;
        private CellViewModel viewModel;
        public ChessCell()
        {
            InitializeComponent();

            Panel.SetZIndex(cellBtnSelect, 2);
            Panel.SetZIndex(imgPiece, 1);

            this.Width = this.Height;
            this.MaxWidth = this.MaxHeight;
            this.MinWidth = this.MinHeight;
            var imageUrl = $"/Graphics/Selectors/Red Marker.png";
            var resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
            imgCheckMarker.Source = new BitmapImage(resourceUri);
            cellBtnSelect.Style = this.FindResource("defaultRepeatBtn") as Style;
            cellBtnMove.Style = this.FindResource("defaultBtn") as Style;
            cellBtnPromote.Style = this.FindResource("defaultBtn") as Style;
            pieceRegex = new Regex(@"(White|Black) [A-Za-z]+");
        }

        private void ChessCell_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = this.DataContext as CellViewModel;
            selectCommand = viewModel!.SelectCommand;
        }

        private void Unselect()
        {
            if (imgPiece.Source != null)
            {
                selectCommand.Execute(null);
            }
        }

        private void cellBtnSelect_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (cellBtnSelect.IsEnabled)
            {
                Panel.SetZIndex(cellBtnSelect, 2);
            }
            else
            {
                Panel.SetZIndex(cellBtnSelect, 1);
            }
        }

        private void cellBtnMove_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (cellBtnMove.IsEnabled)
            {
                Panel.SetZIndex(cellBtnMove, 3);
                cellBtnMove.Opacity = 0.5;
                imgSelector.Opacity = 1;
                Panel.SetZIndex(imgSelector, 2);
            }
            else
            {
                Panel.SetZIndex(cellBtnMove, 1);
                cellBtnMove.Opacity = 0;
                imgSelector.Opacity = 0;
                Panel.SetZIndex(imgSelector, 1);
            }
        }

        private void cellBtnPromote_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (cellBtnPromote.IsEnabled)
            {
                Panel.SetZIndex(cellBtnPromote, 2);
                cellBackground.Background = Brushes.White;
                cellBackground.Opacity = 1;
                imgPiece.Opacity = 1;
            }
            else
            {
                Panel.SetZIndex(cellBtnPromote, 1);
                cellBackground.Background = null;
            }
        }

        private void imgCheckMarker_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (imgCheckMarker.IsEnabled)
            {
                Panel.SetZIndex(imgCheckMarker, 1);
                imgCheckMarker.Opacity = 1;
            }
            else
            {
                Panel.SetZIndex(imgCheckMarker, 0);
                imgCheckMarker.Opacity = 0.0;
            }
        }

        private void cellBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (imgSelector.Source != null && imgSelector.Source.ToString().Contains("Occupied"))
            {
                imgSelector.Height = this.ActualHeight;
                imgSelector.Width = this.ActualWidth;
            }
            else
            {
                imgSelector.Height = GlobalDimensions.Height / 24;
                imgSelector.Width = GlobalDimensions.Width / 48;
            }
            imgPiece.Height = this.ActualHeight - 1;
            imgPiece.Width = this.ActualWidth - 1;
        }

        private void imgSelector_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (imgSelector.Source != null && imgSelector.Source.ToString().Contains("Occupied"))
            {
                imgSelector.Height = this.ActualHeight;
                imgSelector.Width = this.ActualWidth;
                imgSelector.Stretch = Stretch.Fill;
            }
            else
            {
                imgSelector.Height = GlobalDimensions.Height / 24;
                imgSelector.Width = GlobalDimensions.Width / 48;
                imgSelector.Stretch = Stretch.Uniform;
            }
        }

        private void cellBtnSelect_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (cellBtnSelect.IsEnabled)
            {
                selectCommand.Execute(null);
                if (viewModel.IsSelected)
                {
                    var cursorImage = pieceRegex.Match(imgPiece.Source.ToString());
                    Mouse.OverrideCursor = new Cursor(Application.GetResourceStream(new Uri($"Graphics/Cursors/{cursorImage}.cur", UriKind.Relative)).Stream);
                    imgPiece.Opacity = 0.0;
                    DragDrop.DoDragDrop(this, new DataObject(DataFormats.Serializable, this), DragDropEffects.Move);
                    Mouse.OverrideCursor = Cursors.Arrow;
                    imgPiece.Opacity = 1;
                }
            }
        }

        private void cellBtnMove_Drop(object sender, DragEventArgs e)
        {
            if (cellBtnMove.IsEnabled)
            {
                cellBtnMove.Command.Execute(null);
            }
        }
    }
}
