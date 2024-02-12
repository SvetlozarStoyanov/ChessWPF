using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Boards;
using ChessWPF.Models.Moves;
using ChessWPF.Models.Options;
using ChessWPF.Models.Pieces.Enums;
using ChessWPF.Models.Positions;
using ChessWPF.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessWPF.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private string moveNotation;
        private bool canCopyMoveNotation;
        private const string regexPattern = @"[0-9]{1}[\/]{0,1}[0-9]{0,1}-[0-9]{1}[\/]{0,1}[0-9]{0,1}";
        private Regex regex = new Regex(regexPattern);
        private BoardViewModel boardViewModel;
        private GameMenuViewModel menuViewModel;
        private Dictionary<string, GameClockViewModel> gameClockViewModels;

        public GameViewModel(GameStateStore gameStateStore)
        {
            SetupBoardViewModel(gameStateStore.CurrentPosition);
            SetupGameMenuViewModel(BoardViewModel);
            SetupGameClockViewModels(gameStateStore.GameOptions.TimeControl);
            StartGame();
            NavigateToMainMenuCommand = new NavigateCommand<MainMenuViewModel>(gameStateStore, () => new MainMenuViewModel(gameStateStore));
        }

        public string MoveNotation
        {
            get => moveNotation;
            set
            {
                moveNotation = value;
                OnPropertyChanged(nameof(MoveNotation));
            }
        }

        public bool CanCopyMoveNotation
        {
            get => canCopyMoveNotation;
            set
            {
                canCopyMoveNotation = value;
                OnPropertyChanged(nameof(CanCopyMoveNotation));
            }
        }

        public Board Board
        {
            get => BoardViewModel.Board;
        }
        public BoardViewModel BoardViewModel
        {
            get => boardViewModel;
            set => boardViewModel = value;
        }

        public GameMenuViewModel GameMenuViewModel
        {
            get => menuViewModel;
            set => menuViewModel = value;
        }


        public Dictionary<string, GameClockViewModel> GameClockViewModels
        {
            get => gameClockViewModels;
            private set => gameClockViewModels = value;
        }

        public ICommand NavigateToMainMenuCommand { get; init; }

        public void StartGame()
        {
            BoardViewModel.StartGame();
            UpdateClocks();
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.EndGame();
                GameMenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                GameMenuViewModel.UpdateGameStatus($"{Board.TurnColor} to play");
                GameClockViewModels[Board.TurnColor.ToString()].StartClock();
            }
        }

        public void ResetBoard(object sender, EventArgs args)
        {
            BoardViewModel.ResetBoard();
            ResetMoveAnnotation();
            ResetClocks();
            StartGame();
        }

        public void MovePiece(object sender, MovedToCellViewModelEventArgs args)
        {
            GameClockViewModels[Board.TurnColor.ToString()].StopClock();
            var currTurnColor = Board.TurnColor;
            BoardViewModel.MovePiece(args.Cell);

            if (Board.Moves.Any() && !Board.Moves.Peek().IsPromotionMove && Board.OngoingPromotionMove == null)
            {
                GameClockViewModels[currTurnColor.ToString()].AddIncrement();
                AddToMoveAnnotation(Board.Moves.Peek());
            }
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.EndGame();
                GameMenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                GameMenuViewModel.UpdateGameStatus($"{Board.TurnColor} to play");
                GameClockViewModels[Board.TurnColor.ToString()].StartClock();
            }
        }

        public void UndoMove(object sender, EventArgs args)
        {
            if (BoardViewModel.Board.OngoingPromotionMove == null)
            {
                GameClockViewModels[Board.TurnColor.ToString()].StopClock();
                GameClockViewModels[Board.TurnColor.ToString()].AddTime(GameClockViewModels[Board.TurnColor.ToString()].TimeElapsed);
                RemoveFromMoveAnnotation();
            }

            BoardViewModel.UndoMove();
            GameMenuViewModel.UpdateGameStatus($"{Board.TurnColor} to play");
            GameClockViewModels[Board.TurnColor.ToString()].StartClock();
        }

        public void SelectPieceForPromotion(object sender, PromotePieceEventArgs args)
        {
            GameClockViewModels[Board.TurnColor.ToString()].StopClock();
            GameClockViewModels[Board.TurnColor.ToString()].AddIncrement();
            BoardViewModel.PromotePiece(args.PieceType);
            AddToMoveAnnotation(Board.Moves.Peek());
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.EndGame();
                GameMenuViewModel.UpdateGameStatus(BoardViewModel.GameResult);
            }
            else
            {
                GameMenuViewModel.UpdateGameStatus($"{Board.TurnColor} to play");
                GameClockViewModels[Board.TurnColor.ToString()].StartClock();
            }
        }

        public void EndGameByTimeOut(PieceColor color)
        {
            GameMenuViewModel.UpdateGameStatus($"{color.ToString()} wins by timeout!");
            BoardViewModel.EndGameByTimeOut(color);
        }

        public void ResetClocks()
        {
            foreach (var clock in gameClockViewModels.Values)
            {
                clock.StopClock();
                clock.ResetClock();
            }
        }

        public void UpdateClocks()
        {
            foreach (var clock in gameClockViewModels)
            {
                clock.Value.UpdateClock(clock.Value.GameClock.StartingTime);
            }
        }

        private void SetupBoardViewModel(Position position)
        {
            BoardViewModel = new BoardViewModel(position);
            BoardViewModel.MovedPiece += MovePiece;
            BoardViewModel.UndoLastMove += UndoMove;
            BoardViewModel.Reset += ResetBoard;
            BoardViewModel.Promote += SelectPieceForPromotion;
        }

        private void SetupGameMenuViewModel(BoardViewModel boardViewModel)
        {
            GameMenuViewModel = new GameMenuViewModel(boardViewModel);
        }

        private void AddToMoveAnnotation(Move move)
        {
            var sb = new StringBuilder(MoveNotation);
            sb.Append($"{(move.CellOneBefore.Piece!.Color == PieceColor.White ?
                ($"{Board.Moves.Count(m => m.CellOneBefore.Piece!.Color == PieceColor.White) + 1 / 2}.")
                : string.Empty)}{move.Annotation} ");
            MoveNotation = sb.ToString();
            CanCopyMoveNotation = true;
        }

        private void RemoveFromMoveAnnotation()
        {
            var sb = new StringBuilder(MoveNotation.TrimEnd());
            var lastSpaceIndex = MoveNotation.TrimEnd().LastIndexOf(" ");
            if (lastSpaceIndex == -1)
            {
                ResetMoveAnnotation();
                return;
            }
            var temp = MoveNotation.Substring(lastSpaceIndex, sb.Length - lastSpaceIndex);
            var match = regex.Match(temp);
            if (match.Success)
            {
                sb = sb.Remove(lastSpaceIndex, sb.Length - lastSpaceIndex);
                MoveNotation = sb.Append(" ").ToString();
            }
            lastSpaceIndex = MoveNotation.TrimEnd().LastIndexOf(" ");

            if (lastSpaceIndex == -1)
            {
                ResetMoveAnnotation();
                return;
            }
            sb = sb.Remove(lastSpaceIndex, sb.Length - lastSpaceIndex);
            MoveNotation = sb.Append(" ").ToString();
        }

        private void ResetMoveAnnotation()
        {
            MoveNotation = string.Empty;
            CanCopyMoveNotation = false;
        }

        private void OnTimeOut(object source, TimeOutEventArgs args)
        {
            var oppositeColor = args.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            EndGameByTimeOut(oppositeColor);
        }

        private void SetupGameClockViewModels(TimeControl timeControl)
        {
            var whiteGameClockViewModel = new GameClockViewModel(PieceColor.White,
                Color.FromRgb(0, 100, 0),
                Color.FromRgb(255, 255, 255),
                Color.FromRgb(255, 0, 0),
                timeControl);

            var blackGameClockViewModel = new GameClockViewModel(PieceColor.Black,
                Color.FromRgb(0, 100, 0),
                Color.FromRgb(255, 255, 255),
                Color.FromRgb(255, 0, 0),
                timeControl);
            var gameClockViewModels = new Dictionary<string, GameClockViewModel>()
            {
                ["White"] = whiteGameClockViewModel,
                ["Black"] = blackGameClockViewModel
            };
            GameClockViewModels = gameClockViewModels;
            foreach (var clock in gameClockViewModels)
            {
                clock.Value.TimeOut += OnTimeOut;
            }
        }
    }
}
