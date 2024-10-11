using ChessWPF.HelperClasses.CustomEventArgs;
using System;

namespace ChessWPF.ViewModels.Moves
{
    public class MoveGridViewModel : ViewModelBase
    {
        private MoveBoxViewModel movesTree;

        public MoveGridViewModel()
        {

        }

        public MoveBoxViewModel MovesTree
        {
            get { return movesTree; }
            set { movesTree = value; }
        }

        public event ChangeSelectedMoveEventHandler ChangeSelectedMove;
        public delegate void ChangeSelectedMoveEventHandler (object? sender, ChangeSelectedMoveEventArgs e);

        public void AddMove(MoveBoxViewModel moveBoxViewModel)
        {
            MovesTree?.Children.Add(moveBoxViewModel);
            moveBoxViewModel.Parent = MovesTree!;
            moveBoxViewModel.SelectMove += SelectMove;
            MovesTree = moveBoxViewModel;
        }

        public void SelectMove(object sender, EventArgs e)
        {
            if (sender == null)
            {
                throw new NullReferenceException();
            }

            var movesBoxViewModel = sender as MoveBoxViewModel;

            MovesTree.IsSelected = false;

            movesBoxViewModel.IsSelected = true;

            MovesTree = movesBoxViewModel;

            ChangeSelectedMove?.Invoke(null, new ChangeSelectedMoveEventArgs(movesBoxViewModel.Move));
        }
    }
}
