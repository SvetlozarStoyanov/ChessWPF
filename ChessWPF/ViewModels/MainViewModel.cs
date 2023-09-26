using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Singleton;
using System.Collections.Generic;

namespace ChessWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public GameViewModel GameViewModel { get; set; }
        public MainViewModel()
        {
            BoardViewModel boardViewModel = new BoardViewModel();
            MenuViewModel menuViewModel = new MenuViewModel(boardViewModel);
            GameClockViewModel whiteGameClockViewModel = new GameClockViewModel(PieceColor.White);
            GameClockViewModel blackGameClockViewModel = new GameClockViewModel(PieceColor.Black);
            var gameClocks = new Dictionary<string, GameClockViewModel>()
            {
                ["White"] = whiteGameClockViewModel,
                ["Black"] = blackGameClockViewModel
            };
            GameViewModel = new GameViewModel(boardViewModel, menuViewModel, gameClocks);
            BackgroundSingleton.Instance.GameViewModel = GameViewModel;
            BackgroundSingleton.Instance.StartGame();
        }
    }
}
