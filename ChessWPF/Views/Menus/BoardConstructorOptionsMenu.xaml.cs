using ChessWPF.ViewModels;
using System;
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
            if (!checkbox!.IsEnabled)
            {
                checkbox!.IsChecked = false;
                //checkbox.Command?.Execute(null);
            }
        }

        private void turnColorsComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            turnColorsComboBox.Items.Clear();
            turnColorsComboBox.ItemsSource = (DataContext as BoardConstructorMenuViewModel)!.TurnColors;
        }

        private void enPassantComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            enPassantComboBox.Items.Clear();
            enPassantComboBox.ItemsSource = (DataContext as BoardConstructorMenuViewModel)!.EnPassantPossibilities;
        }

        private void resetBoardBtn_Click(object sender, RoutedEventArgs e)
        {
            //Console.Clear();
        }

        private void clearBoardBtn_Click(object sender, RoutedEventArgs e)
        {
            //Console.Clear();
        }

        private void loadLastSavedPositionBtn_Click(object sender, RoutedEventArgs e)
        {
            //Console.Clear();
        }
    }
}
