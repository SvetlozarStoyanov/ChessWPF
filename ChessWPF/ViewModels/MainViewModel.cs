using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;

namespace ChessWPF.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        public GameViewModel GameViewModel { get; set; }
        public MainViewModel()
        {
            var boardViewModel = new BoardViewModel();
            var menuViewModel = new MenuViewModel(boardViewModel);
            var whiteGameClockViewModel = new GameClockViewModel(PieceColor.White);
            var blackGameClockViewModel = new GameClockViewModel(PieceColor.Black);
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
