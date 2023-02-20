using ChessWPF.Models.Data.Board;

namespace ChessWPF.Models.Data.Pieces
{
    public abstract class Piece
    {
        protected Piece(string name, string color)
        {
            Name = name;
            Color = color;

        }

        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public Cell Position { get; set; }
    }
}