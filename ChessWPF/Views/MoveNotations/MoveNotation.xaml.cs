using ChessWPF.HelperClasses.WindowDimensions;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.MoveNotations
{
    /// <summary>
    /// Interaction logic for MoveNotation.xaml
    /// </summary>
    public partial class MoveNotation : UserControl
    {
        public MoveNotation()
        {
            InitializeComponent();
        }

        private void copyMoveNotationButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(moveNotationTextBox.Text);
        }

        private void moveNotationGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            moveNotationGrid.Height = GlobalDimensions.Height / 4;
        }

        private void moveNotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            moveNotationTextBox.ScrollToEnd();
        }
    }
}
