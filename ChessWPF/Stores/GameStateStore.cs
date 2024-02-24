using ChessWPF.Game;
using ChessWPF.Models.Options;
using ChessWPF.Models.Positions;
using ChessWPF.ViewModels;
using System;

namespace ChessWPF.Stores
{
    public class GameStateStore
    {
        private ViewModelBase currentViewModel;
        private GameOptions gameOptions;
        private string currentPositionFenAnnotation;



        public GameStateStore()
        {
            GameOptions = new GameOptions();
            CurrentPositionFenAnnotation = PositionCreator.CreateDefaultPositionFenAnnotation();
        }

        public event Action CurrentViewModelChanged;
        public ViewModelBase CurrentViewModel
        {
            get => currentViewModel;
            set
            {
                currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }


        public GameOptions GameOptions
        {
            get { return gameOptions; }
            set { gameOptions = value; }
        }

        public string CurrentPositionFenAnnotation
        {
            get => currentPositionFenAnnotation;
            private set => currentPositionFenAnnotation = value;
        }

        public void UpdateCurrentPositionFenAnnotation(string fenAnnotation)
        {
            CurrentPositionFenAnnotation = fenAnnotation;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
