using ChessWPF.HelperClasses.WindowDimensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessWPF.Views.Clocks
{
    /// <summary>
    /// Interaction logic for ChessCell.xaml
    /// </summary>
    public partial class GameClock : UserControl
    {

        public GameClock()
        {
            InitializeComponent();
        }

        private void GameClock_Loaded(object sender, RoutedEventArgs e)
        {
            var clockTimeSize = GlobalDimensions.Width / 64;
            timeTextBlock.FontSize = clockTimeSize;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var clockTimeSize = GlobalDimensions.Width / 64;
            timeTextBlock.FontSize = clockTimeSize;
        }
    }
}
