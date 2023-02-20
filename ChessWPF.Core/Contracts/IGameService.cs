using ChessWPF.Infrastructure.Data.Board;

namespace ChessWPF.Core.Contracts
{
    public interface IGameService
    {
        IEnumerable<Cell> GetLegalMoves<T>(Cell Cell, string color);
    }
}
