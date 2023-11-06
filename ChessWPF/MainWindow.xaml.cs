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
            this.MaxWidth = PrimaryScreenWidth;
            this.MinWidth = PrimaryScreenWidth / 2;
            this.MaxHeight = PrimaryScreenHeight;
            this.MinHeight = PrimaryScreenHeight / 2;
            InitializeComponent();
        }
    }
}
