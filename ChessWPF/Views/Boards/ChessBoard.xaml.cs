using ChessWPF.HelperClasses.ControlGetters;
using ChessWPF.HelperClasses.WindowDimensions;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Boards
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoard : UserControl
    {
        private List<TextBlock> cellAnnotationTextBlocks;

        public ChessBoard()
        {
            InitializeComponent();
            fenTextBox.FocusVisualStyle = null;
            cellAnnotationTextBlocks = ControlChildElementsFetcher.GetChildrenOfType<TextBlock>(grid);
        }

        private void ChessBoard_Loaded(object sender, RoutedEventArgs e)
        {
            var fenFontSize = GlobalDimensions.Width / 100;
            var cellAnnotationFontSize = GlobalDimensions.Width / 64;
            fenTextBox.FontSize = fenFontSize;
            cellAnnotationTextBlocks.ForEach(block => block.FontSize = cellAnnotationFontSize);
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var fenFontSize = GlobalDimensions.Width / 100;
            var cellAnnotationFontSize = GlobalDimensions.Width / 64;
            fenTextBox.FontSize = fenFontSize;
            cellAnnotationTextBlocks.ForEach(block => block.FontSize = cellAnnotationFontSize);
        }

        private void cellGrid_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeChessCells();
        }

        private void InitializeChessCells()
        {
            var cellViewModels = (this.DataContext as BoardViewModel)!.CellViewModels;
            for (int row = 0; row < cellViewModels.Length; row++)
            {
                for (int col = 0; col < cellViewModels[row].Length; col++)
                {
                    var chessCell = new ChessCell();
                    chessCell.SetValue(Grid.RowProperty, row);
                    chessCell.SetValue(Grid.ColumnProperty, col);
                    chessCell.DataContext = cellViewModels[row][col];
                    cellGrid.Children.Add(chessCell);
                }
            }
        }
    }
}
