using ChessWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Menus
{
    /// <summary>
    /// Interaction logic for BoardConstructorOptionsMenu.xaml
    /// </summary>
    public partial class BoardConstructorOptionsMenu : UserControl
    {
        public BoardConstructorOptionsMenu()
        {
            InitializeComponent();
        }

        private void CheckBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            checkbox!.IsChecked = false;
            checkbox.Command?.Execute(null);
        }

        private void turnColorsComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            turnColorsComboBox.Items.Clear();
            turnColorsComboBox.ItemsSource = (DataContext as BoardConstructorMenuViewModel)!.TurnColors;
        }
    }
}
