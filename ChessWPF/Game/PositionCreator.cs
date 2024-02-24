using ChessWPF.Models.Positions;

namespace ChessWPF.Game
{
    public static class PositionCreator
    {
        public static string CreateDefaultPositionFenAnnotation()
        {
            var fenAnnotation = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            

            return fenAnnotation;
        }
    }
}
