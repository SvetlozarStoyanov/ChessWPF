using ChessWPF.Commands;
using ChessWPF.Models.Data.Enums;
using ChessWPF.Models.Data.Options;
using ChessWPF.Stores;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class GameOptionsViewModel : ViewModelBase
    {
        private GameOptions gameOptions;

        public GameOptionsViewModel(GameStateStore gameStateStore)
        {
            gameOptions = gameStateStore.GameOptions;
            NavigateToMainMenuCommand = new NavigateCommand<MainMenuViewModel>(gameStateStore, () => new MainMenuViewModel(gameStateStore));
            InitializeTimeControls();
        }

        public GameOptions GameOptions
        {
            get { return gameOptions; }
            private set { gameOptions = value; }
        }

        public ICommand NavigateToMainMenuCommand { get; init; }
        public ObservableCollection<TimeControlViewModel> TimeControlViewModels { get; private set; }

        private void InitializeTimeControls()
        {
            TimeControlViewModels = new ObservableCollection<TimeControlViewModel>
            {
                new TimeControlViewModel(new TimeControl(30, 0, TimeControlType.Bullet)),
                new TimeControlViewModel(new TimeControl(60, 0, TimeControlType.Bullet)),
                new TimeControlViewModel(new TimeControl(120, 1, TimeControlType.Bullet)),
                new TimeControlViewModel(new TimeControl(180, 2, TimeControlType.Blitz)),
                new TimeControlViewModel(new TimeControl(300, 5, TimeControlType.Blitz)),
                new TimeControlViewModel(new TimeControl(300, 0, TimeControlType.Blitz)),
                new TimeControlViewModel(new TimeControl(600, 5, TimeControlType.Rapid)),
                new TimeControlViewModel(new TimeControl(900, 10, TimeControlType.Rapid)),
                new TimeControlViewModel(new TimeControl(900, 0, TimeControlType.Rapid)),
                new TimeControlViewModel(new TimeControl(1800, 0, TimeControlType.Classical)),
                new TimeControlViewModel(new TimeControl(1800, 20, TimeControlType.Classical)),
                new TimeControlViewModel(new TimeControl(3600, 30, TimeControlType.Classical))
            };
            TimeControlViewModels
                .FirstOrDefault(tc => tc.TimeControl.ClockTime == GameOptions.TimeControl.ClockTime
                && tc.TimeControl.ClockIncrement == GameOptions.TimeControl.ClockIncrement)!.IsSelected = true;
            foreach (var timeControlViewModel in TimeControlViewModels)
            {
                timeControlViewModel.Select += SelectedTimeControlChanged;
            }
        }

        private void SelectedTimeControlChanged()
        {
            GameOptions.TimeControl = TimeControlViewModels.FirstOrDefault(tc => tc.IsSelected)!.TimeControl;
        }
    }
}
