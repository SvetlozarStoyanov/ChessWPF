using ChessWPF.Models.Data.Board;
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
            if (cellBtnSelect.IsEnabled)
            {
                Panel.SetZIndex(cellBtnSelect, 2);
                Panel.SetZIndex(cellBtnMove, 1);
            }
            string imageUrl = $"/Graphics/Selectors/Green Marker.png";
            Uri resourceUri = new Uri(@$"pack://application:,,,{imageUrl}");
            imgSelector.Source = new BitmapImage(resourceUri);
            imgSelector.Height = 30;
            imgSelector.Width = 30;
            //imgSelector.HorizontalAlignment = HorizontalAlignment.Center;
            //imgSelector.VerticalAlignment = VerticalAlignment.Center;
            imgSelector.Opacity = 0;

        }
        private void cellBtnSelect_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (cellBtnSelect.IsEnabled)
            {
                Panel.SetZIndex(cellBtnSelect, 2);
                Panel.SetZIndex(cellBtnMove, 1);
            }
            else
            {
                Panel.SetZIndex(cellBtnSelect, 1);
                Panel.SetZIndex(cellBtnMove, 2);
            }

        }

        private void cellBtnMove_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (cellBtnMove.IsEnabled)
            {
                Panel.SetZIndex(cellBtnSelect, 1);
                Panel.SetZIndex(cellBtnMove, 2);
                cellBtnMove.Opacity = 0.5;
                cellBtnMove.Background = Brushes.DarkSlateGray;
                imgSelector.Opacity = 1;
            }
            else
            {
                Panel.SetZIndex(cellBtnSelect, 2);
                Panel.SetZIndex(cellBtnMove, 1);
                cellBtnMove.Opacity = 0;
                cellBtnMove.Background = null;
                imgSelector.Opacity = 0;

            }

        }
    }
}
