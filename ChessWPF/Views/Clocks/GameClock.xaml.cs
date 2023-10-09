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
            double clockTimeSize = ((SystemParameters.PrimaryScreenWidth / 6) / 3 * 2) / 5 * 0.7;
            Application.Current.Resources.Remove("ClockTimeFontSize");
            Application.Current.Resources.Add("ClockTimeFontSize", clockTimeSize);
            double clockColorSize = ((SystemParameters.PrimaryScreenWidth / 5) / 3 * 2) / 5 * 0.7;
            Application.Current.Resources.Remove("ClockColorFontSize");
            Application.Current.Resources.Add("ClockColorFontSize", clockColorSize);
        }
    }
}
