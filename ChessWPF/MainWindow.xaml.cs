using ChessWPF.HelperClasses.WindowDimensions;
using System.Windows;
using static System.Windows.SystemParameters;

namespace ChessWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MaxWidth = PrimaryScreenWidth;
            this.MinWidth = PrimaryScreenWidth / 2;
            this.MaxHeight = PrimaryScreenHeight;
            this.MinHeight = PrimaryScreenHeight / 2;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GlobalDimensions.Height = this.ActualHeight;
            GlobalDimensions.Width = this.ActualWidth;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GlobalDimensions.Height = this.ActualHeight;
            GlobalDimensions.Width = this.ActualWidth;
        }
    }
}
