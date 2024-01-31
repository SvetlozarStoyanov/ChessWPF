using ChessWPF.ViewModels;

namespace ChessWPF.Commands
{
    public class UpdateConstructorCellCommand : CommandBase
    {
        private readonly ConstructorCellViewModel constructorCellViewModel;

        public UpdateConstructorCellCommand(ConstructorCellViewModel constructorCellViewModel)
        {
            this.constructorCellViewModel = constructorCellViewModel;
        }

        public override void Execute(object? parameter)
        {
            constructorCellViewModel.UpdateCellPiece();
        }
    }
}
