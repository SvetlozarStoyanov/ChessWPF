using ChessWPF.HelperClasses.WindowDimensions;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Menus
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class GameMenu : UserControl
    {
        public GameMenu()
        {
            InitializeComponent();
            gameStatusMenuItem.FontWeight = FontWeights.Bold;
        }

        private void Menu_Loaded(object sender, RoutedEventArgs e)
        {
            var menuFontSize = GlobalDimensions.Width / 64;
            newGameMenuItem.FontSize = menuFontSize;
            undoMoveMenuItem.FontSize = menuFontSize;
            navigateToMainMenuMenuItem.FontSize = menuFontSize;
            gameStatusMenuItem.FontSize = menuFontSize;
        }

        private void Menu_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var menuFontSize = GlobalDimensions.Width / 64;
            newGameMenuItem.FontSize = menuFontSize;
            undoMoveMenuItem.FontSize = menuFontSize;
            navigateToMainMenuMenuItem.FontSize = menuFontSize;
            gameStatusMenuItem.FontSize = menuFontSize;
        }
    }
}
