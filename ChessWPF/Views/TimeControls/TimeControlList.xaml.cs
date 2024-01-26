using ChessWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessWPF.Views.TimeControls
{
    /// <summary>
    /// Interaction logic for TimeControlList.xaml
    /// </summary>
    public partial class TimeControlList : UserControl
    {
        public TimeControlList()
        {
            InitializeComponent();
            
        }

        private void ItemsControl_Loaded(object sender, RoutedEventArgs e)
        {
            var list = (this.DataContext as List<TimeControlViewModel>);
            itemsControl.ItemsSource = list;
            timeControlTypeLabel.Content = list.First().TimeControl.TimeControlType;
        }
    }
}
