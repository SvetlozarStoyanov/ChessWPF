using ChessWPF.HelperClasses.WindowDimensions;
using System.Runtime.InteropServices;
using System.Windows;
using static System.Windows.SystemParameters;

namespace ChessWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //[DllImport("Kernel32")]
        //public static extern void AllocConsole();
        public MainWindow()
        {
            InitializeComponent();
            this.MaxWidth = PrimaryScreenWidth;
            this.MinWidth = PrimaryScreenWidth / 2;
            this.MaxHeight = PrimaryScreenHeight;
            this.MinHeight = PrimaryScreenHeight / 2;
            //AllocConsole();
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
