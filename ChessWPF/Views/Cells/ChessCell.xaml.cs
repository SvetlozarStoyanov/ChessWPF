using System.Windows.Controls;
using System.Windows.Media;

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
                cellBtnMove.Opacity = 0.25;
                cellBtnMove.Background = Brushes.Green;
            }
            else
            {
                Panel.SetZIndex(cellBtnSelect, 2);
                Panel.SetZIndex(cellBtnMove, 1);
                cellBtnMove.Opacity = 0;
                cellBtnMove.Background = null;
            }

        }
    }
}
