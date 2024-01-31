using ChessWPF.HelperClasses.ControlGetters;
using ChessWPF.HelperClasses.WindowDimensions;
using ChessWPF.ViewModels;
using ChessWPF.Views.Cells;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ChessWPF.Views.Boards
{
    /// <summary>
    /// Interaction logic for ConstructorBoard.xaml
    /// </summary>
    public partial class ConstructorBoard : UserControl
    {
        private List<TextBlock> cellAnnotationTextBlocks;

        public ConstructorBoard()
        {
            InitializeComponent();
            cellAnnotationTextBlocks = ControlChildElementsFetcher.GetChildrenOfType<TextBlock>(mainGrid);
        }

        private void mainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var cellAnnotationFontSize = GlobalDimensions.Width / 64;
            cellAnnotationTextBlocks.ForEach(block => block.FontSize = cellAnnotationFontSize);
        }

        private void mainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var cellAnnotationFontSize = GlobalDimensions.Width / 64;
            cellAnnotationTextBlocks.ForEach(block => block.FontSize = cellAnnotationFontSize);
        }

        private void cellGrid_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeChessCells();
        }

        private void InitializeChessCells()
        {
            var constructorCellViewModels = (this.DataContext as ConstructorCellViewModel[][])!;
            for (int row = 0; row < constructorCellViewModels.Length; row++)
            {
                for (int col = 0; col < constructorCellViewModels[row].Length; col++)
                {
                    var chessCell = new ChessConstructorCell();
                    chessCell.SetValue(Grid.RowProperty, row);
                    chessCell.SetValue(Grid.ColumnProperty, col);
                    chessCell.DataContext = constructorCellViewModels[row][col];
                    cellGrid.Children.Add(chessCell);
                }
            }
        }


    }
}
