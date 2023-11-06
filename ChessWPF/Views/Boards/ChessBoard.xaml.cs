using ChessWPF.HelperClasses.WindowDimensions;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Boards
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoard : UserControl
    {
        public ChessBoard()
        {
            InitializeComponent();
            fenTextBox.FocusVisualStyle = null;
        }

        private void ChessBoard_Loaded(object sender, RoutedEventArgs e)
        {
            var fenFontSize = GlobalDimensions.Width / 100;
            fenTextBox.FontSize = fenFontSize;
        }

        private void gameResultTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (gameResultTextBox.IsEnabled)
            {
                MessageBox.Show(gameResultTextBox.Text, "Game over!");
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var fenFontSize = GlobalDimensions.Width / 100;
            fenTextBox.FontSize = fenFontSize;
        }
    }
}
