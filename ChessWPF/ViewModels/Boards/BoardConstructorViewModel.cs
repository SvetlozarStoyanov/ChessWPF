using ChessWPF.Commands;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Stores;
using System.Windows.Input;
using System.Windows.Media;

namespace ChessWPF.ViewModels
{
    public class BoardConstructorViewModel : ViewModelBase
    {
        private ConstructorPiece? selectedPiece;
        private readonly BoardConstructor boardConstructor;
        private readonly GameStateStore gameStateStore;
        private readonly BoardConstructorMenuViewModel boardConstructorMenuViewModel;
        private ConstructorCellViewModel[][] constructorCellViewModels;

        public BoardConstructorViewModel(GameStateStore gameStateStore)
        {
            this.gameStateStore = gameStateStore;
            BoardConstructor = new BoardConstructor();
            MatchCellViewModelsToCells();
            BoardConstructor.ImportPosition(gameStateStore.CurrentPosition);
            NavigateToMainMenuCommand = new NavigateCommand<MainMenuViewModel>(gameStateStore, () => new MainMenuViewModel(gameStateStore));
            NavigateToGameCommand = new NavigateCommand<GameViewModel>(gameStateStore, () => new GameViewModel(gameStateStore));
            BoardConstructorMenuViewModel = new BoardConstructorMenuViewModel(gameStateStore);
        }

        public ConstructorPiece? SelectedPiece
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

        public ConstructorCellViewModel[][] ConstructorCellViewModels
        {
            get { return constructorCellViewModels; }
            set { constructorCellViewModels = value; }
        }

        public ICommand NavigateToMainMenuCommand { get; init; }
        public ICommand NavigateToGameCommand { get; init; }

        private void MatchCellViewModelsToCells()
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
                        MatchCellViewModelToCell(row, col, evenTileColor);
                    }
                    else
                    {
                        MatchCellViewModelToCell(row, col, oddTileColor);
                    }
                }
            }
        }

        private void MatchCellViewModelToCell(int row, int col, Color color)
        {
            ConstructorCellViewModels[row][col] = new ConstructorCellViewModel(BoardConstructor.ConstructorCells[row, col], color);
        }
    }
}
