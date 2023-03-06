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
            SetupPieces();
            //SetupPiecesDemo();
            //SetupPiecesCheckBishopTest();
            //SetupPiecesCheckRookTest();
            //SetupPiecesEnPassantTest();
            //SetupPiecesPromotionTest();
            //SetupPiecesStalemateTest();
            //SetupPiecesKnightCheckTest();
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

        private void SetupPieces()
        {
            SetupKings();
            SetupPawns();
            SetupQueens();
            SetupRooks();
            SetupBishops();
            SetupKnights();
        }

        private void SetupKings()
        {
            Cells[0, 4].Piece = new King(PieceColor.Black, Cells[0, 4]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
        }

        private void SetupQueens()
        {
            Cells[0, 3].Piece = new Queen(PieceColor.Black, Cells[0, 3]);

            Cells[7, 3].Piece = new Queen(PieceColor.White, Cells[7, 3]);
        }

        private void SetupBishops()
        {
            Cells[0, 2].Piece = new Bishop(PieceColor.Black, Cells[0, 2]);
            Cells[0, 5].Piece = new Bishop(PieceColor.Black, Cells[0, 5]);

            Cells[7, 2].Piece = new Bishop(PieceColor.White, Cells[7, 2]);
            Cells[7, 5].Piece = new Bishop(PieceColor.White, Cells[7, 5]);
        }

        private void SetupKnights()
        {
            Cells[0, 1].Piece = new Knight(PieceColor.Black, Cells[0, 1]);
            Cells[0, 6].Piece = new Knight(PieceColor.Black, Cells[0, 6]);

            Cells[7, 1].Piece = new Knight(PieceColor.White, Cells[7, 1]);
            Cells[7, 6].Piece = new Knight(PieceColor.White, Cells[7, 6]);
        }

        private void SetupPawns()
        {
            int row = 1;
            for (int col = 0; col < Cells.GetLength(0); col++)
            {
                Cells[row, col].Piece = new Pawn(PieceColor.Black, Cells[row, col]);
            }
            row = 6;
            for (int col = 0; col < Cells.GetLength(0); col++)
            {
                Cells[row, col].Piece = new Pawn(PieceColor.White, Cells[row, col]);
            }
        }

        private void SetupRooks()
        {
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);
            Cells[0, 7].Piece = new Rook(PieceColor.Black, Cells[0, 7]);

            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[7, 0].Piece = new Rook(PieceColor.White, Cells[7, 0]);
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

        private void SetupPiecesPromotionTest()
        {
            Cells[1, 1].Piece = new Pawn(PieceColor.White, Cells[1, 1]);
            Cells[1, 6].Piece = new Pawn(PieceColor.White, Cells[1, 6]);

            Cells[6, 1].Piece = new Pawn(PieceColor.Black, Cells[6, 1]);
            Cells[6, 7].Piece = new Pawn(PieceColor.Black, Cells[6, 7]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[1, 4].Piece = new King(PieceColor.Black, Cells[1, 4]);
        }

        private void SetupPiecesStalemateTest()
        {
            Cells[6, 7].Piece = new Pawn(PieceColor.Black, Cells[6, 7]);

            //Cells[2, 6].Piece = new Queen(PieceColor.White, Cells[2, 6]);
            Cells[7, 7].Piece = new Bishop(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Bishop(PieceColor.Black, Cells[0, 0]);
            //Cells[6, 2].Piece = new Knight(PieceColor.Black, Cells[6, 2]);

            Cells[6, 6].Piece = new King(PieceColor.White, Cells[6, 6]);
            Cells[1, 4].Piece = new King(PieceColor.Black, Cells[1, 4]);

        }

        private void SetupPiecesKnightCheckTest()
        {
            Cells[6, 3].Piece = new Knight(PieceColor.White, Cells[6, 3]);
            Cells[7, 5].Piece = new Queen(PieceColor.White, Cells[7, 5]);

            Cells[3, 5].Piece = new Pawn(PieceColor.Black, Cells[3, 5]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[2, 5].Piece = new King(PieceColor.Black, Cells[2, 5]);


        }
    }
}
