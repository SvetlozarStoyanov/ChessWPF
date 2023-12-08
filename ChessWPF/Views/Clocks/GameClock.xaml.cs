using ChessWPF.HelperClasses.WindowDimensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            var clockColorSize = GlobalDimensions.Width / 53;
            //colorTextBlock.FontSize = clockColorSize;
            if (clockGrid.IsEnabled)
            {
                clockGrid.Background = Brushes.Green;
            }
            else
            {
                clockGrid.Background = Brushes.White;
            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var clockTimeSize = GlobalDimensions.Width / 64;
            timeTextBlock.FontSize = clockTimeSize;
            var clockColorSize = GlobalDimensions.Width / 53;
            //colorTextBlock.FontSize = clockColorSize;
        }

        private void clockGrid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (clockGrid.IsEnabled)
            {
                clockGrid.Background = Brushes.Green;
            }
            else
            {
                clockGrid.Background = Brushes.White;
            }
        }
    }
}
