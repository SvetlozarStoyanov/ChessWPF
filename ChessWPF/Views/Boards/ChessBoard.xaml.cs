using ChessWPF.HelperClasses.ControlGetters;
using ChessWPF.HelperClasses.WindowDimensions;
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
    }
}
