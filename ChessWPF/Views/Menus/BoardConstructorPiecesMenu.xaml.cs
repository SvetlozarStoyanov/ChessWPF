using ChessWPF.ViewModels.Pieces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessWPF.Views.Menus
{
    /// <summary>
    /// Interaction logic for BoardConstructorPiecesMenu.xaml
    /// </summary>
    public partial class BoardConstructorPiecesMenu : UserControl
    {
        public BoardConstructorPiecesMenu()
        {
            InitializeComponent();
            Panel.SetZIndex(deletePieceRadioButton, 2);
            Panel.SetZIndex(deleteImage, 1);           
            Panel.SetZIndex(selectPieceRadioButton, 2);
            Panel.SetZIndex(selectImage, 1);
            var deleteIconUrl = "/Graphics/Icons/BoardConstructor/Trash.png";
            deleteImage.Source = new BitmapImage(new Uri(@$"pack://application:,,,{deleteIconUrl}"));
            var selectIconUrl = "/Graphics/Icons/BoardConstructor/Pointer.png";
            selectImage.Source = new BitmapImage(new Uri(@$"pack://application:,,,{selectIconUrl}"));
        }

        private void constructorPiecesItemControl_Loaded(object sender, RoutedEventArgs e)
        {
            var pieces = this.DataContext as HashSet<ConstructorPieceViewModel>;
            constructorPiecesItemControl.ItemsSource = pieces;
        }

        private void selectPieceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            selectPieceRadioButton.Background = Brushes.Green;
        }

        private void selectPieceRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            selectPieceRadioButton.Background = null;
        }

        private void deletePieceRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            deletePieceRadioButton.Background = Brushes.Red;
        }

        private void deletePieceRadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            deletePieceRadioButton.Background = null;
        }
    }
}
