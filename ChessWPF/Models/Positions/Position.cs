using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System.Collections.Generic;

namespace ChessWPF.Models.Positions
{
    public sealed class Position
    {
        private string fenAnnotation;
        private PieceColor turnColor;
        private Dictionary<PieceColor, List<Piece>> pieces;

        public Position()
        {
            
        }

        public string FenAnnotation
        {
            get { return fenAnnotation; }
            set { fenAnnotation = value; }
        }

        public PieceColor TurnColor
        {
            get { return turnColor; }
            set { turnColor = value; }
        }

        public Dictionary<PieceColor, List<Piece>> Pieces
        {
            get { return pieces; }
            set { pieces = value; }
        }

        private void UpdateFenAnnotation()
        {

        }
    }
}
