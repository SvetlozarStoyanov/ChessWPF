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

            string imageUrl = $"/Graphics/Selectors/Green Marker.png";
            Uri resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
            imgSelector.Source = new BitmapImage(resourceUri);
            imageUrl = $"/Graphics/Selectors/Red Marker.png";
            resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
            imgSelector.Height = 30;
            imgSelector.Width = 30;
            imgCheckMarker.Source = new BitmapImage(resourceUri);
            imgSelector.Opacity = 0;
            //imgCheckMarker.Opacity = 0;
            imgCheckMarker.Height = 85;
            imgCheckMarker.Width = 85;
            imgCheckMarker.HorizontalAlignment = HorizontalAlignment.Center;
            imgCheckMarker.VerticalAlignment = VerticalAlignment.Center;
            cellBtnSelect.Style = this.FindResource("defaultBtn") as Style;
            cellBtnMove.Style = this.FindResource("defaultBtn") as Style;
            cellBtnPromote.Style = this.FindResource("defaultBtn") as Style;
            checkBtn.Style = this.FindResource("defaultBtn") as Style;
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
            if (cellBtnSelect.Opacity == 0.6)
            {
                cellBtnSelect.Opacity = 0;
                cellBtnSelect.Style = this.FindResource("defaultBtn") as Style;
            }
            else
            {
                cellBtnSelect.Opacity = 0.6;
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

        private void checkBtn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (checkBtn.IsEnabled)
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
    }
}
