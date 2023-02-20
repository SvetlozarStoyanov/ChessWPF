using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;

namespace ChessWPF.Models.Data.Board
{
    public class Board
    {
        private Cell[,] cells;
        private Stack<Move> moves;
        private Dictionary<PieceColor, List<Piece>> pieces;



        public Board()
        {
            Cells = new Cell[8, 8];
            Moves = new Stack<Move>();
            Pieces = new Dictionary<PieceColor, List<Piece>>
            {
                { PieceColor.White, new List<Piece>() },
                { PieceColor.Black, new List<Piece>() }
            };
            CreateCells(Cells);
            SetupPiecesDemo();
            //SetupPiecesCheckBishopTest();
            //SetupPiecesCheckRookTest();
            //SetupPiecesEnPassantTest();
        }
        public Cell[,] Cells { get { return cells; } set { cells = value; } }
        public Stack<Move> Moves
        {
            get { return moves; }
            set { moves = value; }
        }

        public Dictionary<PieceColor, List<Piece>> Pieces
        {
            get { return pieces; }
            set { pieces = value; }
        }


        public void CreateCells(Cell[,] cells)
        {
            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int col = 0; col < cells.GetLength(1); col++)
                {
                    cells[row, col] = new Cell(row, col);
                }
            }
        }

        private void SetupPiecesDemo()
        {
            Cells[6, 3].Piece = new Pawn(PieceColor.White, Cells[6, 3]);
            Cells[6, 4].Piece = new Pawn(PieceColor.White, Cells[6, 4]);

            Cells[1, 3].Piece = new Pawn(PieceColor.Black, Cells[1, 3]);
            Cells[1, 4].Piece = new Pawn(PieceColor.Black, Cells[1, 4]);

            Cells[6, 6].Piece = new Bishop(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new Bishop(PieceColor.Black, Cells[1, 6]);

            Cells[4, 4].Piece = new Knight(PieceColor.White, Cells[4, 4]);
            Cells[3, 3].Piece = new Knight(PieceColor.Black, Cells[3, 3]);

            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);

            Cells[6, 2].Piece = new Queen(PieceColor.White, Cells[6, 2]);
            Cells[3, 4].Piece = new Queen(PieceColor.Black, Cells[3, 4]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[0, 4].Piece = new King(PieceColor.Black, Cells[0, 4]);

        }

        private void SetupPiecesCheckBishopTest()
        {
            Cells[6, 3].Piece = new Bishop(PieceColor.White, Cells[6, 3]);
            Cells[0, 2].Piece = new Bishop(PieceColor.Black, Cells[0, 2]);

            Cells[6, 6].Piece = new King(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new King(PieceColor.Black, Cells[1, 6]);
        }

        private void SetupPiecesCheckRookTest()
        {
            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);

            Cells[6, 6].Piece = new King(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new King(PieceColor.Black, Cells[1, 6]);
        }
        private void SetupPiecesEnPassantTest()
        {
            Cells[3, 2].Piece = new Pawn(PieceColor.White, Cells[3, 2]);
            Cells[6, 6].Piece = new Pawn(PieceColor.White, Cells[6, 6]);

            Cells[1, 1].Piece = new Pawn(PieceColor.Black, Cells[1, 1]);
            Cells[4, 5].Piece = new Pawn(PieceColor.Black, Cells[4, 5]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[1, 4].Piece = new King(PieceColor.Black, Cells[1, 4]);
        }
    }
}
