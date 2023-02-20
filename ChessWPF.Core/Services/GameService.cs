using ChessWPF.Core.Contracts;
using ChessWPF.Infrastructure.Data.Board;
using ChessWPF.Infrastructure.Data.Pieces;

namespace ChessWPF.Core.Services
{
    public class GameService : IGameService
    {
        private Board board;
        public GameService(Board gameBoard)
        {
            this.board = gameBoard;
        }
        public IEnumerable<Cell> GetLegalMoves<T>(Cell cell, string color)
        {
            if (typeof(T) == typeof(Pawn))
            {
                if (color == "White")
                {
                    if (cell.Row < 8)
                    {
                        if (board.Cells[cell.Row - 1, cell.Col].Piece == null)
                        {
                            return new List<Cell>(cell.Row - 1);
                        }
                    }
                }
                else
                {
                    if (cell.Row > 1)
                    {
                        if (board.Cells[cell.Row + 1, cell.Col].Piece == null)
                        {
                            return new List<Cell>(cell.Row + 1);
                        }
                    }
                }
            }
            return null;
        }
    }
}
