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

        public static List<ValueTuple<Piece, Piece>> FindDefendersSimplified(King king,
            PieceColor turnColor,
            Dictionary<PieceColor, List<Piece>> pieces,
            char[,] simplifiedCells)
        {
            var oppositeColor = turnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            if (pieces[oppositeColor].Any(p => p.PieceType == PieceType.Rook || p.PieceType == PieceType.Queen || p.PieceType == PieceType.Knook))
            {
                defenders.AddRange(CheckRowsAndColsSimplified(king, simplifiedCells, pieces));
            }
            if (pieces[oppositeColor].Any(p => p.PieceType == PieceType.Bishop || p.PieceType == PieceType.Queen))
            {
                defenders.AddRange(CheckDiagonalsSimplified(king, simplifiedCells, pieces));
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
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var pinningPiece = currCell.Piece;
                        var isDefender = false;
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
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row - increment, king.Col, board))
            {
                currCell = board.Cells[king.Row - increment, king.Col];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var isDefender = false;
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
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row, king.Col + increment, board))
            {
                currCell = board.Cells[king.Row, king.Col + increment];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var pinningPiece = currCell.Piece;
                        var isDefender = false;
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
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row, king.Col - increment, board))
            {
                currCell = board.Cells[king.Row, king.Col - increment];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var pinningPiece = currCell.Piece;
                        var isDefender = false;
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
                    }
                    break;
                }
                increment++;
            }
            return defenders;
        }

        private static List<ValueTuple<Piece, Piece>> CheckRowsAndColsSimplified(King king,
            char[,] cells,
            Dictionary<PieceColor, List<Piece>> pieces)
        {
            var oppositeColor = king.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            var potentialAttackers = king.Color == PieceColor.White ? new char[] { 'r', 'q', 'o' } : new char[] { 'R', 'Q', 'O' };
            var potentialAttackerTypes = new PieceType[] { PieceType.Rook, PieceType.Queen, PieceType.Knook };
            Predicate<char> isAttacker = letter => char.IsLetter(letter) && potentialAttackers.Contains(letter);
            Predicate<char> isSameColorAsKing = letter => char.IsLetter(letter)
            && (king.Color == PieceColor.White ? char.IsUpper(letter)
            : char.IsLower(letter));
            var currCell = cells[king.Row, king.Col];
            var defenderCell = cells[king.Row, king.Col];
            int increment = 1;
            if (pieces[oppositeColor].Any(p => potentialAttackerTypes.Contains(p.PieceType) && p.Col == king.Col))
            {
                while (IsCellValid(king.Row + increment, king.Col, cells))
                {
                    currCell = cells[king.Row + increment, king.Col];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row + increment;
                        var defenderCol = king.Col;
                        defenderCell = currCell;

                        while (IsCellValid(king.Row + ++increment, king.Col, cells))
                        {
                            currCell = cells[king.Row + increment, king.Col];
                            if (isAttacker(currCell))
                            {
                                var pinningPiece = currCell;
                                var pinningPieceRow = king.Row + increment;
                                var pinningPieceCol = king.Col;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), (GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces))));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                              && cells[king.Row + increment, king.Col] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
                increment = 1;

                while (IsCellValid(king.Row - increment, king.Col, cells))
                {
                    currCell = cells[king.Row - increment, king.Col];
                    if (isSameColorAsKing(currCell))
                    {
                        defenderCell = currCell;
                        var defenderRow = king.Row - increment;
                        var defenderCol = king.Col;

                        while (IsCellValid(king.Row - ++increment, king.Col, cells))
                        {
                            currCell = cells[king.Row - increment, king.Col];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row - increment;
                                var pinningPieceCol = king.Col;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), (GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces))));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                              && cells[king.Row - increment, king.Col] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
                increment = 1;
            }
            if (pieces[oppositeColor].Any(p => potentialAttackerTypes.Contains(p.PieceType) && p.Row == king.Row))
            {
                while (IsCellValid(king.Row, king.Col + increment, cells))
                {
                    currCell = cells[king.Row, king.Col + increment];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row;
                        var defenderCol = king.Col;
                        defenderCell = currCell;

                        while (IsCellValid(king.Row, king.Col + ++increment, cells))
                        {
                            currCell = cells[king.Row, king.Col + increment];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row;
                                var pinningPieceCol = king.Col + increment;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), (GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces))));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                              && cells[king.Row, king.Col + increment] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
                increment = 1;

                while (IsCellValid(king.Row, king.Col - increment, cells))
                {
                    currCell = cells[king.Row, king.Col - increment];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row;
                        var defenderCol = king.Col - increment;
                        defenderCell = currCell;

                        while (IsCellValid(king.Row, king.Col - ++increment, cells))
                        {
                            currCell = cells[king.Row, king.Col - increment];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row;
                                var pinningPieceCol = king.Col - increment;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), (GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces))));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                                && cells[king.Row, king.Col - increment] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
            }
            return defenders;
        }

        private static List<ValueTuple<Piece, Piece>> CheckDiagonals(King king, Board board)
        {
            var increment = 1;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            var pinningPieceTypes = new PieceType[] { PieceType.Bishop, PieceType.Queen };

            var currCell = board.Cells[king.Row, king.Col];
            var defenderCell = board.Cells[king.Row, king.Col];
            while (IsCellValid(king.Row + increment, king.Col + increment, board))
            {
                currCell = board.Cells[king.Row + increment, king.Col + increment];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var pinningPiece = currCell.Piece;
                        var isDefender = false;
                        while (IsCellValid(king.Row + ++increment, king.Col + increment, board))
                        {
                            currCell = board.Cells[king.Row + increment, king.Col + increment];
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
                    }
                    break;
                }

                increment++;
            }
            increment = 1;
            while (IsCellValid(king.Row + increment, king.Col - increment, board))
            {
                currCell = board.Cells[king.Row + increment, king.Col - increment];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        if (currCell.Piece.Color == king.Color)
                        {
                            defenderCell = currCell;
                            var pinningPiece = currCell.Piece;
                            var isDefender = false;
                            while (IsCellValid(king.Row + ++increment, king.Col - increment, board))
                            {
                                currCell = board.Cells[king.Row + increment, king.Col - increment];
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
                        }
                        break;
                    }
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row - increment, king.Col - increment, board))
            {
                currCell = board.Cells[king.Row - increment, king.Col - increment];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var pinningPiece = currCell.Piece;

                        bool isDefender = false;
                        while (IsCellValid(king.Row - ++increment, king.Col - increment, board))
                        {
                            currCell = board.Cells[king.Row - increment, king.Col - increment];
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
                    }
                    break;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(king.Row - increment, king.Col + increment, board))
            {
                currCell = board.Cells[king.Row - increment, king.Col + increment];
                if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color == king.Color)
                    {
                        defenderCell = currCell;
                        var pinningPiece = currCell.Piece;

                        var isDefender = false;
                        while (IsCellValid(king.Row - ++increment, king.Col + increment, board))
                        {
                            currCell = board.Cells[king.Row - increment, king.Col + increment];
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
                    }
                    break;
                }
                increment++;
            }
            return defenders;
        }

        private static List<ValueTuple<Piece, Piece>> CheckDiagonalsSimplified(King king,
            char[,] cells,
            Dictionary<PieceColor, List<Piece>> pieces)
        {
            var oppositeColor = king.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var defenders = new List<ValueTuple<Piece, Piece>>();
            var potentialAttackers = king.Color == PieceColor.White ? new char[] { 'b', 'q' } : new char[] { 'B', 'Q' };
            var potentialAttackerTypes = new PieceType[] { PieceType.Bishop, PieceType.Queen };
            Predicate<char> isAttacker = letter => char.IsLetter(letter) && potentialAttackers.Contains(letter);
            Predicate<char> isSameColorAsKing = letter => char.IsLetter(letter)
            && (king.Color == PieceColor.White ? char.IsUpper(letter)
            : char.IsLower(letter));
            var defenderCell = cells[king.Row, king.Col];
            if (pieces[oppositeColor].Any(p => potentialAttackerTypes.Contains(p.PieceType)))
            {
                var currCell = cells[king.Row, king.Col];
                var increment = 1;
                while (IsCellValid(king.Row + increment, king.Col + increment, cells))
                {
                    currCell = cells[king.Row + increment, king.Col + increment];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row + increment;
                        var defenderCol = king.Col + increment;
                        defenderCell = currCell;
                        while (IsCellValid(king.Row + ++increment, king.Col + increment, cells))
                        {
                            currCell = cells[king.Row + increment, king.Col + increment];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row + increment;
                                var pinningPieceCol = king.Col + increment;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces)));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                                && cells[king.Row + increment, king.Col + increment] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
                increment = 1;

                while (IsCellValid(king.Row + increment, king.Col - increment, cells))
                {
                    currCell = cells[king.Row + increment, king.Col - increment];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row + increment;
                        var defenderCol = king.Col - increment;
                        defenderCell = currCell;
                        while (IsCellValid(king.Row + ++increment, king.Col - increment, cells))
                        {
                            currCell = cells[king.Row + increment, king.Col - increment];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row + increment;
                                var pinningPieceCol = king.Col - increment;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces)));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                                && cells[king.Row + increment, king.Col - increment] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
                increment = 1;

                while (IsCellValid(king.Row - increment, king.Col - increment, cells))
                {
                    currCell = cells[king.Row - increment, king.Col - increment];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row - increment;
                        var defenderCol = king.Col - increment;
                        defenderCell = currCell;
                        while (IsCellValid(king.Row - ++increment, king.Col - increment, cells))
                        {
                            currCell = cells[king.Row - increment, king.Col - increment];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row - increment;
                                var pinningPieceCol = king.Col - increment;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces)));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                                && cells[king.Row - increment, king.Col - increment] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
                increment = 1;

                while (IsCellValid(king.Row - increment, king.Col + increment, cells))
                {
                    currCell = cells[king.Row - increment, king.Col + increment];
                    if (isSameColorAsKing(currCell))
                    {
                        var defenderRow = king.Row - increment;
                        var defenderCol = king.Col + increment;
                        defenderCell = currCell;
                        while (IsCellValid(king.Row - ++increment, king.Col + increment, cells))
                        {
                            currCell = cells[king.Row - increment, king.Col + increment];
                            if (isAttacker(currCell))
                            {
                                var pinningPieceRow = king.Row - increment;
                                var pinningPieceCol = king.Col + increment;
                                var pinningPiece = currCell;
                                defenders.Add((GetPieceFromLetterAndCoordinates(defenderRow, defenderCol, defenderCell, pieces), GetPieceFromLetterAndCoordinates(pinningPieceRow, pinningPieceCol, pinningPiece, pieces)));
                                break;
                            }
                            else if ((isSameColorAsKing(currCell) || !isAttacker(currCell))
                                && cells[king.Row - increment, king.Col + increment] != '.')
                            {
                                break;
                            }
                        }
                        break;
                    }
                    increment++;
                }
            }
            return defenders;
        }

        private static bool IsCellValid(int row, int col, Board board)
        {
            return row >= 0 && row < board.Cells.GetLength(0) && col >= 0 && col < board.Cells.GetLength(1);
        }

        private static bool IsCellValid(int row, int col, char[,] cells)
        {
            return row >= 0 && row < cells.GetLength(0) && col >= 0 && col < cells.GetLength(1);
        }

        private static Piece GetPieceFromLetterAndCoordinates(int row, int col, char letter, Dictionary<PieceColor, List<Piece>> pieces)
        {
            var color = char.IsUpper(letter) ? PieceColor.White : PieceColor.Black;

            var piece = pieces[color].FirstOrDefault(p => p.HasEqualCoordinates(row, col));
            return piece!;
        }
    }
}
