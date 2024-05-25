using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Cells
{
    /// <summary>
    /// Interaction logic for ChessConstructorCell.xaml
    /// </summary>
    public partial class ChessConstructorCell : UserControl
    {
        public ChessConstructorCell()
        {
            InitializeComponent();

            Panel.SetZIndex(imgPiece, 1);
            Panel.SetZIndex(selectCellPieceBtn, 2);


            this.Width = this.Height;
            this.MaxWidth = this.MaxHeight;
            this.MinWidth = this.MinHeight;
            updateCellBtn.Style = this.FindResource("defaultBtn") as Style;
            selectCellPieceBtn.Style = this.FindResource("defaultBtn") as Style;
        }

        private void ChessConstructorCell_Loaded(object sender, RoutedEventArgs e)
        {
            selectCommand = (this.DataContext as ConstructorCellViewModel)!.SelectPieceFromCellCommand;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            imgPiece.Height = this.ActualHeight - 1;
            imgPiece.Width = this.ActualWidth - 1;
        }

        private void selectCellPieceBtn_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (selectCellPieceBtn.IsEnabled)
            {
                updateCellBtn.IsEnabled = false;
                Panel.SetZIndex(selectCellPieceBtn, 2);
                Panel.SetZIndex(updateCellBtn, -1);
            }
            else
            {
                updateCellBtn.IsEnabled = true;
                Panel.SetZIndex(selectCellPieceBtn, -1);
                Panel.SetZIndex(updateCellBtn, 2);
            }
        }

        private void updateCellBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void selectCellPieceBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
