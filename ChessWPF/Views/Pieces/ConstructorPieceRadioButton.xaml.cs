using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessWPF.Views.Pieces
{
    /// <summary>
    /// Interaction logic for ConstructorPieceRadioButton.xaml
    /// </summary>
    public partial class ConstructorPieceRadioButton : UserControl
    {
        public ConstructorPieceRadioButton()
        {
            InitializeComponent();
        }

        private void constructorPieceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Background = Brushes.Green;
        }

        private void constructorPieceRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Background = null;
        }
    }
}
