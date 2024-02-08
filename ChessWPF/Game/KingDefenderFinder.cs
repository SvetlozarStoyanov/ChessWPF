using ChessWPF.Models.Boards;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Game
{
    public static class KingDefenderFinder
    {
        public static List<ValueTuple<Piece, Piece>> FindDefenders(King king, PieceColor turnColor, Board board)
        {   
            var oppositeColor = turnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            if (board.Pieces[oppositeColor].Any(p => p.PieceType == PieceType.Rook || p.PieceType == PieceType.Queen || p.PieceType == PieceType.Knook))
            {
                defenders.AddRange(CheckRowsAndCols(king, board));
            }
            if (board.Pieces[oppositeColor].Any(p => p.PieceType == PieceType.Bishop || p.PieceType == PieceType.Queen))
            {
                defenders.AddRange(CheckDiagonals(king, board));
            }
            return defenders;
        }



        private static List<ValueTuple<Piece, Piece>> CheckRowsAndCols(King king, Board board)
        {
            int increment = 1;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            var pinningPieceTypes = new PieceType[] { PieceType.Rook, PieceType.Queen, PieceType.Knook };
            var currCell = board.Cells[king.Row, king.Col];
            var defenderCell = board.Cells[king.Row, king.Col];
            while (IsCellValid(king.Row + increment, king.Col, board))
            {
                currCell = board.Cells[king.Row + increment, king.Col];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;
                    bool isDefender = false;
                    while (IsCellValid(king.Row + ++increment, king.Col, board))
                    {
                        currCell = board.Cells[king.Row + increment, king.Col];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;
                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row - increment, king.Col, board))
            {
                currCell = board.Cells[king.Row - increment, king.Col];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    bool isDefender = false;
                    var pinningPiece = currCell.Piece;
                    while (IsCellValid(king.Row - ++increment, king.Col, board))
                    {
                        currCell = board.Cells[king.Row - increment, king.Col];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;
                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row, king.Col + increment, board))
            {
                currCell = board.Cells[king.Row, king.Col + increment];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;
                    bool isDefender = false;
                    while (IsCellValid(king.Row, king.Col + ++increment, board))
                    {
                        currCell = board.Cells[king.Row, king.Col + increment];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;
                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row, king.Col - increment, board))
            {
                currCell = board.Cells[king.Row, king.Col - increment];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;
                    bool isDefender = false;
                    while (IsCellValid(king.Row, king.Col - ++increment, board))
                    {
                        currCell = board.Cells[king.Row, king.Col - increment];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;
                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                increment++;
            }
            return defenders;
        }

        private static List<ValueTuple<Piece, Piece>> CheckDiagonals(King king, Board board)
        {
            int rowIncrement = 1;
            int colIncrement = 1;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            var pinningPieceTypes = new PieceType[] { PieceType.Bishop, PieceType.Queen };

            var currCell = board.Cells[king.Row, king.Col];
            var defenderCell = board.Cells[king.Row, king.Col];
            while (IsCellValid(king.Row + rowIncrement, king.Col + colIncrement, board))
            {
                currCell = board.Cells[king.Row + rowIncrement, king.Col + colIncrement];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;
                    bool isDefender = false;
                    while (IsCellValid(king.Row + ++rowIncrement, king.Col + ++colIncrement, board))
                    {
                        currCell = board.Cells[king.Row + rowIncrement, king.Col + colIncrement];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;
                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }

                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (IsCellValid(king.Row + rowIncrement, king.Col - colIncrement, board))
            {
                currCell = board.Cells[king.Row + rowIncrement, king.Col - colIncrement];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;

                    bool isDefender = false;
                    while (IsCellValid(king.Row + ++rowIncrement, king.Col - ++colIncrement, board))
                    {
                        currCell = board.Cells[king.Row + rowIncrement, king.Col - colIncrement];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;

                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;

            while (IsCellValid(king.Row - rowIncrement, king.Col - colIncrement, board))
            {
                currCell = board.Cells[king.Row - rowIncrement, king.Col - colIncrement];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;

                    bool isDefender = false;
                    while (IsCellValid(king.Row - ++rowIncrement, king.Col - ++colIncrement, board))
                    {
                        currCell = board.Cells[king.Row - rowIncrement, king.Col - colIncrement];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;

                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;

            while (IsCellValid(king.Row - rowIncrement, king.Col + colIncrement, board))
            {
                currCell = board.Cells[king.Row - rowIncrement, king.Col + colIncrement];
                if (currCell.Piece != null && currCell.Piece.Color == king.Color)
                {
                    defenderCell = currCell;
                    var pinningPiece = currCell.Piece;

                    bool isDefender = false;
                    while (IsCellValid(king.Row - ++rowIncrement, king.Col + ++colIncrement, board))
                    {
                        currCell = board.Cells[king.Row - rowIncrement, king.Col + colIncrement];
                        if (currCell.Piece != null)
                        {
                            if (currCell.Piece.Color != king.Color && pinningPieceTypes.Contains(currCell.Piece.PieceType))
                            {
                                pinningPiece = currCell.Piece;

                                isDefender = true;
                            }
                            break;
                        }
                    }
                    if (isDefender)
                    {
                        defenders.Add((defenderCell.Piece, pinningPiece));
                    }
                    break;
                }
                rowIncrement++;
                colIncrement++;
            }
            return defenders;
        }

        private static bool IsCellValid(int row, int col, Board board)
        {
            return row >= 0 && row < board.Cells.GetLength(0) && col >= 0 && col < board.Cells.GetLength(1);
        }
    }
}
