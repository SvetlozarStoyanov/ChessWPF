using ChessWPF.Models.Positions;

namespace ChessWPF.Game
{
    public static class PositionCreator
    {
        public static Position CreateDefaultPosition()
        {
            var fenAnnotation = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var position = FenAnnotationReader.GetPosition(fenAnnotation);

            return position;
        }
    }
}
