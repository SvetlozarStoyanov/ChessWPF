using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Models.Positions;
using System.Collections.Generic;

namespace ChessWPF.Game
{
    public static class PositionCreator
    {
        public static Position CreateDefaultPosition()
        {
            var fenAnnotation = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            var position = FenAnnotationReader.GetPosition(fenAnnotation);
            position.FenAnnotation = fenAnnotation;

            return position;
        }
    }
}
