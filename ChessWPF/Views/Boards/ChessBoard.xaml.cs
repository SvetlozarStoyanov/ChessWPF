﻿using ChessWPF.Singleton;
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

namespace ChessWPF.Views.Boards
{
    /// <summary>
    /// Interaction logic for ChessBoard.xaml
    /// </summary>
    public partial class ChessBoard : UserControl
    {
        public ChessBoard()
        {
            InitializeComponent();
            //btnGameEnd.IsEnabled = false;
            fenTextBox.FocusVisualStyle = null;

        }

        private void btnGameEnd_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (btnGameEnd.IsEnabled)
            {
                MessageBox.Show(BackgroundSingleton.Instance.BoardViewModel.GameResult, "Game over!");
            }
        }

        private void ChessBoard_Loaded(object sender, RoutedEventArgs e)
        {
            double controlsize = ((SystemParameters.PrimaryScreenWidth / 10) / 3 * 2) / 5 * 0.7;
            Application.Current.Resources.Remove("FenFontSize");
            Application.Current.Resources.Add("FenFontSize", controlsize);
        }
    }
}
