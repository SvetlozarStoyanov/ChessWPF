using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;
using System.Windows.Media;

namespace ChessWPF.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        public GameViewModel GameViewModel { get; set; }
        public MainViewModel()
        {
            var boardViewModel = new BoardViewModel();
            var menuViewModel = new MenuViewModel(boardViewModel);
            var whiteGameClockViewModel = new GameClockViewModel(PieceColor.White,
                Color.FromRgb(0, 100, 0),
                Color.FromRgb(255, 255, 255),
                Color.FromRgb(255, 0, 0));

            var blackGameClockViewModel = new GameClockViewModel(PieceColor.Black,
                Color.FromRgb(0, 100, 0),
                Color.FromRgb(255, 255, 255),
                Color.FromRgb(255, 0, 0));
            var gameClocks = new Dictionary<string, GameClockViewModel>()
            {
                ["White"] = whiteGameClockViewModel,
                ["Black"] = blackGameClockViewModel
            };
            GameViewModel = new GameViewModel(boardViewModel, menuViewModel, gameClocks);
            GameViewModel.StartGame();
        }
    }
}
