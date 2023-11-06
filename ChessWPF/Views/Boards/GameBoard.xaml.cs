using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Boards
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : UserControl
    {
        public GameBoard()
        {
            InitializeComponent();
        }

        private void copyMoveNotationButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(moveNotationTextBox.Text);
        }
    }
}
