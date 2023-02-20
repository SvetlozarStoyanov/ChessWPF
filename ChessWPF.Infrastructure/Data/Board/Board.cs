namespace ChessWPF.Models.Data.Board
{
    public class Board
    {
        public Board()
        {
            Cells = new Cell[8, 8];
        }
        public Cell[,] Cells { get; set; }
    }
}
