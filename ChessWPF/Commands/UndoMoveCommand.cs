using ChessWPF.Singleton;
using ChessWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessWPF.Commands
{
    public class UndoMoveCommand : CommandBase
    {
        private readonly BoardViewModel boardViewModel;
        public UndoMoveCommand(BoardViewModel boardViewModel)
        {
            this.boardViewModel = boardViewModel;
            boardViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

    

        public override bool CanExecute(object? parameter)
        {
            return boardViewModel.Board.Moves.Count > 0;
        }
        public override void Execute(object? parameter)
        {
            BackgroundSingleton.Instance.UndoMove();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(BoardViewModel.GameHasStarted))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
