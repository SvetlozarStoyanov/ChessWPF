using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Game
{
    public static class CheckDirectionFinder
    {
        public static List<Cell> GetLegalMovesToStopCheck(King king, Piece attacker, Board board)
        {
            var validMovesToStopCheck = new List<Cell>();
            var rowDiff = attacker.Cell.Row - king.Cell.Row;
            var colDiff = attacker.Cell.Col - king.Cell.Col;
            switch (attacker.PieceType)
            {
                case PieceType.Pawn:
                    validMovesToStopCheck.AddRange(GetLegalMovesToStopCheckForPawn(king, attacker, board));
                    break;
                case PieceType.Knight:
                    validMovesToStopCheck.Add(attacker.Cell);
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
                    if (king.Cell.Row == attacker.Cell.Row || king.Cell.Col == attacker.Cell.Col)
                    {
                        validMovesToStopCheck = GetLegalMovesToStopCheckForRook(king, attacker, board, rowDiff, colDiff);
                    }
                    else
                    {
                        validMovesToStopCheck.Add(attacker.Cell);
                    }
                    break;
            }
            return validMovesToStopCheck;
        }

        private static List<Cell> GetLegalMovesToStopCheckForPawn(King king, Piece pawn, Board board)
        {
            var validMovesToStopCheck = new List<Cell>();
            validMovesToStopCheck.Add(pawn.Cell);
            var enPassantMoveToStopCheck = new Cell(-1, -1);
            switch (king.Color)
            {
                case PieceColor.White:
                    if (board.Pieces[king.Color].Any(p => p.PieceType == PieceType.Pawn))
                    {
                        validMovesToStopCheck.Add(board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col]);
                    }
                    break;
                case PieceColor.Black:
                    if (board.Pieces[king.Color].Any(p => p.PieceType == PieceType.Pawn))
                    {
                        validMovesToStopCheck.Add(board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col]);
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

            var currCell = bishop.Cell;
            if (rowDiff > 0 && colDiff > 0)
            {
                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Cell.Row - rowIncrement++, bishop.Cell.Col - colIncrement++];
                }
            }
            else if (rowDiff < 0 && colDiff > 0)
            {
                rowDiff = Math.Abs(rowDiff);
                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Cell.Row + rowIncrement++, bishop.Cell.Col - colIncrement++];
                }
            }
            else if (rowDiff < 0 && colDiff < 0)
            {
                rowDiff = Math.Abs(rowDiff);
                colDiff = Math.Abs(colDiff);
                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Cell.Row + rowIncrement++, bishop.Cell.Col + colIncrement++];
                }
            }
            else if (rowDiff > 0 && colDiff < 0)
            {
                colDiff = Math.Abs(colDiff);

                while (rowIncrement <= rowDiff && colIncrement <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[bishop.Cell.Row - rowIncrement++, bishop.Cell.Col + colIncrement++];
                }
            }
            return validMovesToStopCheck;
        }

        private static List<Cell> GetLegalMovesToStopCheckForRook(King king, Piece rook, Board board, int rowDiff, int colDiff)
        {
            var validMovesToStopCheck = new List<Cell>();

            var increment = 1;
            var currCell = rook.Cell;
            if (rowDiff > 0)
            {
                while (increment <= rowDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Cell.Row - increment++, rook.Cell.Col];
                }
            }
            else if (rowDiff < 0)
            {
                rowDiff = Math.Abs(rowDiff);
                while (increment <= rowDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Cell.Row + increment++, rook.Cell.Col];
                }
            }
            else if (colDiff > 0)
            {
                while (increment <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Cell.Row, rook.Cell.Col - increment++];
                }
            }
            else if (colDiff < 0)
            {
                colDiff = Math.Abs(colDiff);

                while (increment <= colDiff)
                {
                    validMovesToStopCheck.Add(currCell);
                    currCell = board.Cells[rook.Cell.Row, rook.Cell.Col + increment++];
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
