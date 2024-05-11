using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class SelectPieceFromCellCommand : CommandBase
    {
        private readonly ConstructorCellViewModel constructorCellViewModel;

        public SelectPieceFromCellCommand(ConstructorCellViewModel constructorCellViewModel)
        {
            this.constructorCellViewModel = constructorCellViewModel;
        }

        public override void Execute(object? parameter)
        {
            constructorCellViewModel.Select();
        }
    }
}
