using ChessWPF.Stores;
using ChessWPF.ViewModels;
using System.Windows;

namespace ChessWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            GameStateStore gameStateStore = new GameStateStore();
            gameStateStore.CurrentViewModel = new MainMenuViewModel(gameStateStore);
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(gameStateStore)
            };
            MainWindow.Show();
            base.OnStartup(e);
        }
    }
}
