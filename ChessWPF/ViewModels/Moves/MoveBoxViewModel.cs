using ChessWPF.Models.Moves;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace ChessWPF.ViewModels.Moves
{
    public class MoveBoxViewModel : ViewModelBase
    {
        private Move move;

        public MoveBoxViewModel(Move move, SolidColorBrush defaultColor, SolidColorBrush selectedColor)
        {
            this.DefaultColor = Brushes.Gray;
            this.SelectedColor = Brushes.LightGreen;
            this.move = move;
        }

        public Move Move
        {
            get { return move; }
            init { move = value; }
        }

        public MoveBoxViewModel Parent { get; set; }
        public List<MoveBoxViewModel> Children { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }
        public SolidColorBrush DefaultColor { get; set; }
        public SolidColorBrush SelectedColor { get; set; }
        public bool IsSelected { get; set; }

        public event SelectMoveEventHandler SelectMove;
        public delegate void SelectMoveEventHandler(object sender, EventArgs e);

        public string Annotation => move.Annotation;

        public void Select()
        {
            SelectMove?.Invoke(this, EventArgs.Empty);
        }
    }
}
