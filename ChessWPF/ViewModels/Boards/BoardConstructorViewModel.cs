using ChessWPF.Commands;
using ChessWPF.Contracts.Pieces;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Stores;
using ChessWPF.ViewModels.Pieces;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessWPF.ViewModels
{
    public class BoardConstructorViewModel : ViewModelBase
    {
        private IConstructorPiece? selectedPiece;
        private readonly BoardConstructor boardConstructor;
        private readonly GameStateStore gameStateStore;
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;
        private ConstructorCellViewModel[][] constructorCellViewModels;
        private Dictionary<PieceColor, HashSet<ConstructorMenuPieceViewModel>> constructorMenuPieceViewModels;

        public BoardConstructorViewModel(GameStateStore gameStateStore)
        {
            GameStateStore = gameStateStore;
            BoardConstructor = new BoardConstructor();
            MatchConstructorCellsToViewModels();
            CreateConstructorCellPieceViewModels();
            BoardConstructor.ImportPosition(gameStateStore.CurrentPosition);
            NavigateToMainMenuCommand = new NavigateCommand<MainMenuViewModel>(gameStateStore, () => new MainMenuViewModel(gameStateStore));
            NavigateToGameCommand = new NavigateCommand<GameViewModel>(gameStateStore, () => new GameViewModel(gameStateStore));
            SelectDeletePieceCommand = new SelectDeletePieceCommand(this);
            SelectPieceSelectorCommand = new SelectPieceSelectorCommand(this);
            BoardConstructorMenuViewModel = new BoardConstructorMenuViewModel();
            EnableSelectingPiecesFromBoard();
        }

        public IConstructorPiece? SelectedPiece
        {
            get { return selectedPiece; }
            private set { selectedPiece = value; }
        }

        public BoardConstructor BoardConstructor
        {
            get { return boardConstructor; }
            init { boardConstructor = value; }
        }

        public BoardConstructorMenuViewModel BoardConstructorMenuViewModel
        {
            get { return boardConstructorMenuViewModel; }
            init { boardConstructorMenuViewModel = value; }
        }

        public GameStateStore GameStateStore
        {
            get => gameStateStore;
            private init => gameStateStore = value;
        }

        public Dictionary<PieceColor, HashSet<ConstructorMenuPieceViewModel>> ConstructorMenuPieceViewModels
        {
            get { return constructorMenuPieceViewModels; }
            private set { constructorMenuPieceViewModels = value; }
        }

        public ConstructorCellViewModel[][] ConstructorCellViewModels
        {
            get { return constructorCellViewModels; }
            set { constructorCellViewModels = value; }
        }

        public ICommand NavigateToMainMenuCommand { get; init; }

        public ICommand NavigateToGameCommand { get; init; }

        public ICommand SelectDeletePieceCommand { get; init; }

        public ICommand SelectPieceSelectorCommand { get; init; }

        public void SelectDeletePiece()
        {
            ChangeSelectedPiece(null);
            DisableSelectingPiecesFromBoard();
        }

        public void EnableSelectingPiecesFromBoard()
        {
            ChangeSelectedPiece(null);
            var flattenedCells = ConstructorCellViewModels.SelectMany(fc => fc).ToList();
            flattenedCells.Where(fc => fc.ConstructorCell.ConstructorBoardPiece != null).ToList().ForEach(fc => fc.UpdateCanBeSelected(this.GetType(), true));
        }

        public void DisableSelectingPiecesFromBoard()
        {
            var flattenedCells = ConstructorCellViewModels.SelectMany(fc => fc).ToList();
            flattenedCells.Where(fc => fc.ConstructorCell.ConstructorBoardPiece != null).ToList().ForEach(fc => fc.UpdateCanBeSelected(this.GetType(), false));
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
                constructorPieceViewModel.SelectConstructorPiece += SelectConstructorPieceChanged;
                viewModels.Add(constructorPieceViewModel);
            }
            return viewModels;
        }

        private void MatchConstructorCellsToViewModels()
        {
            var oddTileColor = (Color)new ColorConverter().ConvertFrom("#14691B")!;
            var evenTileColor = (Color)new ColorConverter().ConvertFrom("#FAE8C8")!;
            ConstructorCellViewModels = new ConstructorCellViewModel[8][];
            for (int row = 0; row < ConstructorCellViewModels.Length; row++)
            {
                ConstructorCellViewModels[row] = new ConstructorCellViewModel[8];
                for (int col = 0; col < ConstructorCellViewModels.Length; col++)
                {
                    if ((row + col) % 2 == 0)
                    {
                        MatchConstructorCellToViewModel(row, col, evenTileColor);
                    }
                    else
                    {
                        MatchConstructorCellToViewModel(row, col, oddTileColor);
                    }
                }
            }
        }

        private void MatchConstructorCellToViewModel(int row, int col, Color color)
        {
            ConstructorCellViewModels[row][col] = new ConstructorCellViewModel(BoardConstructor.ConstructorCells[row, col], color);
            ConstructorCellViewModels[row][col].UpdateConstructorCellViewModel += UpdateConstructionCellViewModel;
            ConstructorCellViewModels[row][col].SelectPieceFromConstructorCellViewModelCell += SelectPieceFromConstructorCellViewModel;
        }

        private void SelectPieceFromConstructorCellViewModel(object? sender, SelectPieceFromConstructorCellViewModelEventArgs e)
        {
            ChangeSelectedPiece(e.ConstructorPiece);
            DisableSelectingPiecesFromBoard();
        }

        private void SelectConstructorPieceChanged(object? sender, SelectMenuPieceEventArgs e)
        {
            ChangeSelectedPiece(e.MenuPiece);
        }

        private void ChangeSelectedPiece(IConstructorPiece? constructorPiece)
        {
            SelectedPiece = constructorPiece;
        }

        private void UpdateConstructionCellViewModel(object? sender, UpdateConstructorCellViewModelEventArgs e)
        {
            BoardConstructor.UpdateCellPiece(e.Row, e.Col, SelectedPiece);

            if (SelectedPiece != null && SelectedPiece is ConstructorBoardPiece)
            {
                var constructorBoardPiece = SelectedPiece as ConstructorBoardPiece;
                if (constructorBoardPiece!.Row != e.Row || constructorBoardPiece.Col != e.Col)
                {
                    BoardConstructor.UpdateCellPiece(constructorBoardPiece.Row, constructorBoardPiece.Col, null);
                    EnableSelectingPiecesFromBoard();
                }
            }
            else
            {
                DisableSelectingPiecesFromBoard();
            }
        }

    }
}
