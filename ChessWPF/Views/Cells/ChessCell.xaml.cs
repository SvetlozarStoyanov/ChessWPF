using ChessWPF.HelperClasses.WindowDimensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.Views.Cells
{
    /// <summary>
    /// Interaction logic for ChessCell.xaml
    /// </summary>
    public partial class ChessCell : UserControl
    {
        public ChessCell()
        {
            InitializeComponent();

            Panel.SetZIndex(cellBtnSelect, 2);
            Panel.SetZIndex(imgPiece, 1);
            this.Width = this.Height;
            this.MaxWidth = this.MaxHeight;
            this.MinWidth = this.MinHeight;
            var imageUrl = $"/Graphics/Selectors/Green Marker.png";
            var resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
            imgSelector.Source = new BitmapImage(resourceUri);
            imgSelector.Height = 40;
            imgSelector.Width = 40;
            imgSelector.MinHeight = 15;
            imgSelector.MinWidth = 15;
            imgSelector.MaxHeight = 40;
            imgSelector.MaxWidth = 40;
            imgSelector.Opacity = 0;
            imageUrl = $"/Graphics/Selectors/Red Marker.png";
            resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
            imgCheckMarker.Source = new BitmapImage(resourceUri);
            cellBtnSelect.Style = this.FindResource("defaultBtn") as Style;
            cellBtnMove.Style = this.FindResource("defaultBtn") as Style;
            cellBtnPromote.Style = this.FindResource("defaultBtn") as Style;
        }

        private void cellBtnSelect_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            cellBtnSelect.Style = this.FindResource("defaultBtn") as Style;
            cellBtnSelect.Opacity = 0;
            if (cellBtnSelect.IsEnabled)
            {
                Panel.SetZIndex(cellBtnSelect, 2);
            }
            else
            {
                Panel.SetZIndex(cellBtnSelect, 1);
            }

        }
        private void cellBtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (cellBtnSelect.Opacity == 0.7)
            {
                cellBtnSelect.Opacity = 0;
                cellBtnSelect.Style = this.FindResource("defaultBtn") as Style;
            }
            else
            {
                cellBtnSelect.Opacity = 0.7;
                cellBtnSelect.Style = this.FindResource("selectedBtn") as Style;
            }
        }

        private void cellBtnSelect_LostFocus(object sender, RoutedEventArgs e)
        {
            cellBtnSelect.Opacity = 0;
            cellBtnSelect.Style = this.FindResource("defaultBtn") as Style;
        }

        private void cellBtnMove_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (cellBtnMove.IsEnabled)
            {
                Panel.SetZIndex(cellBtnMove, 3);
                cellBtnMove.Opacity = 0.5;
                cellBtnMove.Background = Brushes.DarkSlateGray;
                imgSelector.Opacity = 1;
                Panel.SetZIndex(imgSelector, 2);
            }
            else
            {
                Panel.SetZIndex(cellBtnMove, 1);
                cellBtnMove.Opacity = 0;
                cellBtnMove.Background = null;
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
            imgSelector.Width = GlobalDimensions.Width / 48;
            imgSelector.Height = GlobalDimensions.Height / 24;
            imgPiece.Height = this.ActualHeight - 1;
            imgPiece.Width = this.ActualWidth - 1;
        }
    }
}
