using ChessWPF.HelperClasses.WindowDimensions;
using System.Windows.Controls;

namespace ChessWPF.Views.TimeControls
{
    /// <summary>
    /// Interaction logic for TimeControl.xaml
    /// </summary>
    public partial class TimeControl : UserControl
    {
        public TimeControl()
        {
            InitializeComponent();
        }

        //private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        //{
        //    timeControlRadioButton.FontSize = GlobalDimensions.Width / 80;
        //}

        private void timeControlRadioButton_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            timeControlRadioButton.FontSize = GlobalDimensions.Width / 80;
        }
    }
}
