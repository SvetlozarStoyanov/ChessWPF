using ChessWPF.Models.Boards;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Game
{
    public static class LegalMovesToStopCheckFinder
    {
        public static List<Cell> GetLegalMovesToStopCheck(King king, Piece attacker, Board board)
        {
            var validMovesToStopCheck = new List<Cell>();
            var rowDiff = attacker.Row - king.Row;
            var colDiff = attacker.Col - king.Col;
            switch (attacker.PieceType)
            {
                case PieceType.Pawn:
                    validMovesToStopCheck.AddRange(GetLegalMovesToStopCheckForPawn(king, attacker, board));
                    break;
                case PieceType.Knight:
                    validMovesToStopCheck.Add(board.Cells[attacker.Row, attacker.Col]);
                    break;
                case PieceType.Bishop:
                    validMovesToStopCheck.AddRange(GetLegalMovesToStopCheckForBishop(king, attacker, board, rowDiff, colDiff));
                    break;
                case PieceType.Rook:
                    validMovesToStopCheck.AddRange(GetLegalMovesToStopCheckForRook(king, attacker, board, rowDiff, colDiff));
                    break;
                case PieceType.Queen:
                    if (rowDiff != 0 && colDiff != 0)
                    {
                        validMovesToStopCheck = GetLegalMovesToStopCheckForBishop(king, attacker, board, rowDiff, colDiff);
                    }
                    else
                    {
                        validMovesToStopCheck = GetLegalMovesToStopCheckForRook(king, attacker, board, rowDiff, colDiff);
                    }
                    break;
                case PieceType.Knook:
                    if (king.Row == attacker.Row || king.Col == attacker.Col)
                    {
                        validMovesToStopCheck = GetLegalMovesToStopCheckForRook(king, attacker, board, rowDiff, colDiff);
                    }
                    else
                    {
                        validMovesToStopCheck.Add(board.Cells[attacker.Row, attacker.Col]);
                    }
                    break;
            }
            return validMovesToStopCheck;
        }

        private static List<Cell> GetLegalMovesToStopCheckForPawn(King king, Piece pawn, Board board)
        {
            var validMovesToStopCheck = new List<Cell>();
            validMovesToStopCheck.Add(board.Cells[pawn.Row, pawn.Col]);
            var enPassantMoveToStopCheck = new Cell(-1, -1);
            switch (king.Color)
            {
                case PieceColor.White:
                    if (board.Pieces[king.Color].Any(p => p.PieceType == PieceType.Pawn))
                    {
                        validMovesToStopCheck.Add(board.Cells[pawn.Row - 1, pawn.Col]);
                    }
                    break;
                case PieceColor.Black:
                    if (board.Pieces[king.Color].Any(p => p.PieceType == PieceType.Pawn))
                    {
                        validMovesToStopCheck.Add(board.Cells[pawn.Row + 1, pawn.Col]);
                    }
                    break;
            }
            return validMovesToStopCheck;
        }

        private static List<Cell> GetLegalMovesToStopCheckForBishop(King king, Piece bishop, Board board, int rowDiff, int colDiff)
        {
            var validMovesToStopCheck = new List<Cell>();

            var rowIncrement = 1;
            var colIncrement = 1;

            var currCell = board.Cells[bishop.Row, bishop.Col];
            if (rowDiff > 0 && colDiff > 0)
            {
                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Row - rowIncrement++, bishop.Col - colIncrement++];
                }
            }
            else if (rowDiff < 0 && colDiff > 0)
            {
                rowDiff = Math.Abs(rowDiff);
                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Row + rowIncrement++, bishop.Col - colIncrement++];
                }
            }
            else if (rowDiff < 0 && colDiff < 0)
            {
                rowDiff = Math.Abs(rowDiff);
                colDiff = Math.Abs(colDiff);
                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Row + rowIncrement++, bishop.Col + colIncrement++];
                }
            }
            else if (rowDiff > 0 && colDiff < 0)
            {
                colDiff = Math.Abs(colDiff);

                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Row - rowIncrement++, bishop.Col + colIncrement++];
                }
            }
            return validMovesToStopCheck;
        }

        private static List<Cell> GetLegalMovesToStopCheckForRook(King king, Piece rook, Board board, int rowDiff, int colDiff)
        {
            var validMovesToStopCheck = new List<Cell>();

            var increment = 1;
            var currCell = board.Cells[rook.Row, rook.Col];
            if (rowDiff > 0)
            {
                while (increment <= rowDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Row - increment++, rook.Col];
                }
            }
            else if (rowDiff < 0)
            {
                rowDiff = Math.Abs(rowDiff);
                while (increment <= rowDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Row + increment++, rook.Col];
                }
            }
            else if (colDiff > 0)
            {
                while (increment <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Row, rook.Col - increment++];
                }
            }
            else if (colDiff < 0)
            {
                colDiff = Math.Abs(colDiff);

                while (increment <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Row, rook.Col + increment++];
                }
            }
            return validMovesToStopCheck;
        }


        private static bool IsCellValid(int row, int col, Board board)
        {
            return row >= 0 && row < board.Cells.GetLength(0) && col >= 0 && col < board.Cells.GetLength(1);
        }
    }
}
