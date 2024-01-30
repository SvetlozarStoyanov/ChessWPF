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
            updatePieceBtn.Style = this.FindResource("defaultBtn") as Style;
        }
    }
}
