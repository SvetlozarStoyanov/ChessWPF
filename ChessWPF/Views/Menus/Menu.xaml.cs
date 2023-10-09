using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChessWPF.Views.Menus
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        public Menu()
        {
            InitializeComponent();
            //gameStatusMenuItem.Style = this.FindResource("defaultMenuItem") as Style;
            gameStatusMenuItem.FontWeight = FontWeights.Bold;
        }

        private void Menu_Loaded(object sender, RoutedEventArgs e)
        {
            double controlsize = ((SystemParameters.PrimaryScreenWidth / 7) / 3 * 2) / 5 * 0.7;
            Application.Current.Resources.Remove("MenuFontSize");
            Application.Current.Resources.Add("MenuFontSize", controlsize);
        }

    }
}
