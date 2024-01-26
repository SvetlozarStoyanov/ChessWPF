using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Enums;
using ChessWPF.Models.Data.Options;
using ChessWPF.Stores;
using System;
using System.Collections.Generic;
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
            InitializeTimeControlCollections();
        }

        public GameOptions GameOptions
        {
            get { return gameOptions; }
            private set { gameOptions = value; }
        }

        public ICommand NavigateToMainMenuCommand { get; init; }
        public List<TimeControlViewModel> TimeControlViewModels { get; private set; }
        public Dictionary<TimeControlType, List<TimeControlViewModel>> TimeControlViewModelsSplit { get; private set; }

        private void InitializeTimeControlCollections()
        {
            var idNum = (byte)1;
            TimeControlViewModels = new List<TimeControlViewModel>
            {
                new TimeControlViewModel(idNum++, 30, 0, TimeControlType.Bullet),
                new TimeControlViewModel(idNum++, 60, 0, TimeControlType.Bullet),
                new TimeControlViewModel(idNum++, 120, 1, TimeControlType.Bullet),
                new TimeControlViewModel(idNum++, 180, 2, TimeControlType.Blitz),
                new TimeControlViewModel(idNum++, 300, 5, TimeControlType.Blitz),
                new TimeControlViewModel(idNum++, 300, 0, TimeControlType.Blitz),
                new TimeControlViewModel(idNum++, 600, 5, TimeControlType.Rapid),
                new TimeControlViewModel(idNum++, 900, 10, TimeControlType.Rapid),
                new TimeControlViewModel(idNum++, 900, 0, TimeControlType.Rapid),
                new TimeControlViewModel(idNum++, 1800, 0, TimeControlType.Classical),
                new TimeControlViewModel(idNum++, 1800, 20, TimeControlType.Classical),
                new TimeControlViewModel(idNum++, 3600, 30, TimeControlType.Classical)
            };

            TimeControlViewModels.FirstOrDefault(tc => tc.TimeControl.ClockTime == GameOptions.TimeControl.ClockTime
                    && tc.TimeControl.ClockIncrement == GameOptions.TimeControl.ClockIncrement)!.IsSelected = true;

            foreach (var timeControlViewModel in TimeControlViewModels)
            {
                timeControlViewModel.Select += SelectedTimeControlChanged;
            }
            TimeControlViewModelsSplit = new Dictionary<TimeControlType, List<TimeControlViewModel>>();
            foreach (var timeControlType in Enum.GetValues<TimeControlType>())
            {
                TimeControlViewModelsSplit[timeControlType] = TimeControlViewModels
                    .Where(tc => tc.TimeControl.TimeControlType == timeControlType)
                    .ToList();
            }
        }

        private void SelectedTimeControlChanged(object? sender, SelectTimeControlEventArgs eventArgs)
        {
            GameOptions.TimeControl = TimeControlViewModels.FirstOrDefault(tc => tc.Id == eventArgs.Id)!.TimeControl;
        }
    }
}
