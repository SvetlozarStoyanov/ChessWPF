using ChessWPF.Commands;
using ChessWPF.Contracts.Pieces;
using ChessWPF.Game;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Boards;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using ChessWPF.Stores;
using ChessWPF.ViewModels.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessWPF.ViewModels
{
    public sealed class BoardConstructorViewModel : ViewModelBase
    {
        private bool deleteEnabled;
        private bool selectorEnabled = true;
        private IConstructorPiece? selectedPiece;
        private ConstructorCellViewModel selectedConstructorCellViewModel;
        private readonly BoardConstructor boardConstructor;
        private readonly GameStateStore gameStateStore;
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;
        private ConstructorCellViewModel[][] constructorCellViewModels;
        private Dictionary<PieceColor, HashSet<ConstructorMenuPieceViewModel>> constructorMenuPieceViewModels;

        public BoardConstructorViewModel(GameStateStore gameStateStore)
        {
            GameStateStore = gameStateStore;
            BoardConstructor = new BoardConstructor();
            SetupCellConstructorViewModels();
            CreateConstructorCellPieceViewModels();
            BoardConstructor.ImportInProgress = true;
            BoardConstructor.LoadPosition(gameStateStore.CurrentPositionFenAnnotation);
            BoardConstructor.ImportInProgress = false;
            NavigateToMainMenuCommand = new NavigateCommand<MainMenuViewModel>(gameStateStore, () => new MainMenuViewModel(gameStateStore));
            NavigateToGameCommand = new NavigateCommand<GameViewModel>(gameStateStore, () => new GameViewModel(gameStateStore));
            SelectDeletePieceCommand = new SelectDeletePieceCommand(this);
            SelectPieceSelectorCommand = new SelectPieceSelectorCommand(this);
            BoardConstructorMenuViewModel = new BoardConstructorMenuViewModel(
                BoardConstructor.EnPassantCoordinates,
                GetCastlingRightsFromBoardConstructor(),
                BoardConstructor.CastlingPossibilities,
                BoardConstructor.EnPassantPossibilities,
                BoardConstructor.TurnColor);
            BoardConstructor.CastlingPossibilitiesUpdate += UpdateCastlingPossibilities;
            BoardConstructor.CastlingRightsUpdate += UpdateCastlingRightsBackend;
            BoardConstructor.FenAnnotationUpdate += UpdateFenAnnotation;
            BoardConstructor.EnPassantPosibilitiesUpdate += UpdateEnPassantPosibilities;
            BoardConstructor.PositionStatusUpdate += UpdatePositionStatus;
            BoardConstructorMenuViewModel.CastlingRightsUpdate += UpdateCastlingRightsUI;
            BoardConstructorMenuViewModel.TurnColorUpdate += UpdateTurnColor;
            BoardConstructorMenuViewModel.EnPassantCoordinatesUpdate += UpdateEnPassantCoordinates;
            BoardConstructorMenuViewModel.ResetBoardToDefault += ResetBoard;
            BoardConstructorMenuViewModel.LoadLastSavedPosition += LoadLastSavedPosition;
            BoardConstructorMenuViewModel.ClearBoard += ClearBoard;
            BoardConstructorMenuViewModel.SaveCurrentPosition += SaveCurrentPosition;
            EnableSelectingPiecesFromBoard();
        }

        public bool SelectorEnabled
        {
            get => selectorEnabled;
            private set => selectorEnabled = value;
        }

        public bool DeleteEnabled
        {
            get => deleteEnabled;
            private set => deleteEnabled = value;
        }

        public string FenAnnotation
        {
            get => BoardConstructor.FenAnnotation;
            set => OnPropertyChanged(nameof(BoardConstructor.FenAnnotation));
        }

        public IConstructorPiece? SelectedPiece
        {
            get => selectedPiece;
            private set => selectedPiece = value;
        }

        public ConstructorCellViewModel? SelectedConstructorCellViewModel
        {
            get => selectedConstructorCellViewModel;
            private set
            {
                if (selectedConstructorCellViewModel == null)
                {
                    selectedConstructorCellViewModel = value;
                }
                if (value == null && selectedConstructorCellViewModel != null)
                {
                    SelectedConstructorCellViewModel.UnSelect();
                    selectedConstructorCellViewModel = value;
                }
            }
        }

        public BoardConstructor BoardConstructor
        {
            get => boardConstructor;
            init => boardConstructor = value;
        }

        public BoardConstructorMenuViewModel BoardConstructorMenuViewModel
        {
            get => boardConstructorMenuViewModel;
            init => boardConstructorMenuViewModel = value;
        }

        public GameStateStore GameStateStore
        {
            get => gameStateStore;
            private init => gameStateStore = value;
        }

        public Dictionary<PieceColor, HashSet<ConstructorMenuPieceViewModel>> ConstructorMenuPieceViewModels
        {
            get => constructorMenuPieceViewModels;
            private set => constructorMenuPieceViewModels = value;
        }

        public ConstructorCellViewModel[][] ConstructorCellViewModels
        {
            get => constructorCellViewModels;
            private set => constructorCellViewModels = value;
        }

        public ICommand NavigateToMainMenuCommand { get; init; }

        public ICommand NavigateToGameCommand { get; init; }

        public ICommand SelectDeletePieceCommand { get; init; }

        public ICommand SelectPieceSelectorCommand { get; init; }

        public void SelectDeletePiece()
        {
            SelectorEnabled = false;
            DeleteEnabled = true;
            SelectedConstructorCellViewModel = null;
            ChangeSelectedPiece(null);
            DisableSelectingPiecesFromBoard();
        }

        public void SelectPieceSelector()
        {
            SelectorEnabled = true;
            DeleteEnabled = false;
            SelectedConstructorCellViewModel = null;
            EnableSelectingPiecesFromBoard();
        }

        private void ResetBoard(object? sender, EventArgs e)
        {
            LoadPosition(PositionCreator.CreateDefaultPositionFenAnnotation());
        }

        private void LoadLastSavedPosition(object? sender, EventArgs e)
        {
            LoadPosition(gameStateStore.CurrentPositionFenAnnotation);
        }

        private void LoadPosition(string position)
        {
            BoardConstructor.ImportInProgress = true;
            DisableSelectingPiecesFromBoard();
            BoardConstructor.LoadPosition(position);
            if (SelectorEnabled)
            {
                SelectPieceSelector();
            }
            else if (DeleteEnabled)
            {
                SelectDeletePiece();
            }
            BoardConstructorMenuViewModel.SelectedEnPassantCoordinates = BoardConstructor.EnPassantCoordinates;
            BoardConstructorMenuViewModel.SelectedTurnColor = BoardConstructor.TurnColor;
            BoardConstructor.ImportInProgress = false;
        }

        private void ClearBoard(object? sender, EventArgs e)
        {
            if (SelectorEnabled)
            {
                ChangeSelectedPiece(null);
                DisableSelectingPiecesFromBoard();
            }
            BoardConstructor.ClearBoard();
        }

        private void SaveCurrentPosition(object? sender, EventArgs e)
        {
            try
            {
                gameStateStore.UpdateCurrentPositionFenAnnotation(BoardConstructor.FenAnnotation);
                BoardConstructorMenuViewModel.SaveIsEnabled = true;
                BoardConstructorMenuViewModel.PositionErrorText = null;
            }
            catch (Exception ex)
            {
                BoardConstructorMenuViewModel.SaveIsEnabled = false;
                BoardConstructorMenuViewModel.PositionErrorText = ex.Message;
            }
        }

        private void EnableSelectingPiecesFromBoard(int? row = null, int? col = null)
        {
            ChangeSelectedPiece(null);
            var flattenedCells = ConstructorCellViewModels.SelectMany(fc => fc).ToList();
            if (row != null && col != null)
            {
                flattenedCells = flattenedCells
                    .Where(fc => fc.ConstructorCell.Row != row
                    && fc.ConstructorCell.Col != col)
                    .ToList();
            }
            flattenedCells.Where(fc => fc.ConstructorCell.ConstructorBoardPiece != null)
                .ToList()
                .ForEach(fc => fc.UpdateCanBeSelected(this.GetType(), true));
        }

        private void DisableSelectingPiecesFromBoard(int? row = null, int? col = null)
        {
            var flattenedCells = ConstructorCellViewModels
                .SelectMany(fc => fc)
                .ToList();
            if (row != null && col != null)
            {
                flattenedCells = flattenedCells.Where(fc => fc.ConstructorCell.Row != row
                    && fc.ConstructorCell.Col != col).ToList();
            }
            flattenedCells.Where(fc => fc.ConstructorCell.ConstructorBoardPiece != null)
                .ToList()
                .ForEach(fc => fc.UpdateCanBeSelected(this.GetType(), false));
        }

        private bool[] GetCastlingRightsFromBoardConstructor()
        {
            return BoardConstructor.CastlingRights;
        }

        private void UpdateTurnColor(object? sender, TurnColorChangedEventArgs e)
        {
            BoardConstructor.UpdateTurnColor(e.TurnColor);
        }

        private void UpdateCastlingRightsUI(object? sender, EventArgs e)
        {
            BoardConstructor.UpdateCastlingRightsFromUI((sender as BoardConstructorMenuViewModel)!.CastlingRights.ToArray());
        }

        private void UpdateCastlingRightsBackend(object? sender, UpdateCastlingRightsEventArgs e)
        {
            BoardConstructorMenuViewModel.UpdateCastlingRightsBackend(e.CastlingRights);
        }

        private void UpdateCastlingPossibilities(object? sender, EventArgs e)
        {
            BoardConstructorMenuViewModel.UpdateCastlingPossiblities(BoardConstructor.CastlingPossibilities);
        }

        private void UpdateEnPassantPosibilities(object? sender, EventArgs e)
        {
            BoardConstructorMenuViewModel.UpdateEnPassantPosibilities(BoardConstructor.EnPassantPossibilities);
        }

        private void UpdateEnPassantCoordinates(object? sender, EnPassantCoordinatesChangedEventArgs e)
        {
            BoardConstructor.UpdateEnPassantCoordinates(e.CellCoordinates);
        }

        private void UpdateFenAnnotation(object? sender, EventArgs e)
        {
            FenAnnotation = BoardConstructor.FenAnnotation;
        }

        private void UpdatePositionStatus(object? sender, PositionStatusEventArgs e)
        {
            if (e.ErrorMessage == null)
            {
                BoardConstructorMenuViewModel.SaveIsEnabled = true;
                BoardConstructorMenuViewModel.PositionErrorText = null;
            }
            else
            {
                BoardConstructorMenuViewModel.SaveIsEnabled = false;
                BoardConstructorMenuViewModel.PositionErrorText = e.ErrorMessage;
            }
        }

        private void CreateConstructorCellPieceViewModels()
        {
            ConstructorMenuPieceViewModels = new Dictionary<PieceColor, HashSet<ConstructorMenuPieceViewModel>>()
            {
                { PieceColor.White, MatchConstructorPiecesToViewModels(PieceColor.White) },
                { PieceColor.Black, MatchConstructorPiecesToViewModels(PieceColor.Black) }
            };
        }

        private HashSet<ConstructorMenuPieceViewModel> MatchConstructorPiecesToViewModels(PieceColor color)
        {
            var viewModels = new HashSet<ConstructorMenuPieceViewModel>();
            foreach (var piece in BoardConstructor.ConstructorPieces[color])
            {
                var constructorPieceViewModel = new ConstructorMenuPieceViewModel(piece);
                constructorPieceViewModel.SelectConstructorPiece += SelectPieceFromMenuChanged;
                viewModels.Add(constructorPieceViewModel);
            }
            return viewModels;
        }

        private void SetupCellConstructorViewModels()
        {
            var oddTileColor = (Color)new ColorConverter().ConvertFrom("#14691B")!;
            var evenTileColor = (Color)new ColorConverter().ConvertFrom("#FAE8C8")!;
            var oddTileSelectedColor = (Color)new ColorConverter().ConvertFrom("#CBBE11")!;
            var evenTileSelectedColor = (Color)new ColorConverter().ConvertFrom("#E2D313")!;
            ConstructorCellViewModels = new ConstructorCellViewModel[8][];
            for (int row = 0; row < ConstructorCellViewModels.Length; row++)
            {
                ConstructorCellViewModels[row] = new ConstructorCellViewModel[8];
                for (int col = 0; col < ConstructorCellViewModels.Length; col++)
                {
                    if ((row + col) % 2 == 0)
                    {
                        MatchConstructorCellToViewModel(row, col, evenTileColor, evenTileSelectedColor);
                    }
                    else
                    {
                        MatchConstructorCellToViewModel(row, col, oddTileColor, oddTileSelectedColor);
                    }
                }
            }
        }

        private void MatchConstructorCellToViewModel(int row, int col, Color color, Color tileSelectedColor)
        {
            ConstructorCellViewModels[row][col] = new ConstructorCellViewModel(BoardConstructor.ConstructorCells[row, col], color, tileSelectedColor);
            ConstructorCellViewModels[row][col].UpdateConstructorCellViewModel += UpdateConstructionCellViewModel;
            ConstructorCellViewModels[row][col].SelectPieceFromConstructorCellViewModelCell += SelectPieceFromBoardChanged;
        }

        private void SelectPieceFromBoardChanged(object? sender, SelectPieceFromConstructorCellViewModelEventArgs e)
        {

            SelectedConstructorCellViewModel = ConstructorCellViewModels[e.Row][e.Col];
            ChangeSelectedPiece(e.ConstructorPiece);
            DisableSelectingPiecesFromBoard();

        }

        private void SelectPieceFromMenuChanged(object? sender, SelectMenuPieceEventArgs e)
        {
            SelectorEnabled = false;
            DeleteEnabled = false;
            SelectedConstructorCellViewModel = null;
            ChangeSelectedPiece(e.MenuPiece);
            DisableSelectingPiecesFromBoard();
        }

        private void ChangeSelectedPiece(IConstructorPiece? constructorPiece)
        {
            SelectedPiece = constructorPiece;
        }

        private void UpdateConstructionCellViewModel(object? sender, UpdateConstructorCellViewModelEventArgs e)
        {
            if (SelectedPiece != null)
            {
                if (SelectedPiece is ConstructorBoardPiece)
                {
                    var constructorBoardPiece = SelectedPiece as ConstructorBoardPiece;
                    if (constructorBoardPiece!.Row != e.Row || constructorBoardPiece.Col != e.Col)
                    {
                        BoardConstructor.UpdateCellPiece(e.Row, e.Col, SelectedPiece);
                        BoardConstructor.UpdateCellPiece(constructorBoardPiece.Row, constructorBoardPiece.Col, null);
                    }
                    SelectedConstructorCellViewModel = null;
                    EnableSelectingPiecesFromBoard();
                }
                if (SelectedPiece is ConstructorMenuPiece)
                {
                    var constructorMenuPiece = SelectedPiece as ConstructorMenuPiece;
                    if (constructorMenuPiece!.Color == BoardConstructor.ConstructorCells[e.Row, e.Col].ConstructorBoardPiece?.Color
                        && constructorMenuPiece.PieceType == BoardConstructor.ConstructorCells[e.Row, e.Col].ConstructorBoardPiece?.PieceType)
                    {
                        BoardConstructor.UpdateCellPiece(e.Row, e.Col, null);
                    }
                    else
                    {
                        BoardConstructor.UpdateCellPiece(e.Row, e.Col, SelectedPiece);
                    }
                }
            }
            else
            {
                BoardConstructor.UpdateCellPiece(e.Row, e.Col, null);
            }
            BoardConstructor.UpdateFenAnnotation();
        }
    }
}
