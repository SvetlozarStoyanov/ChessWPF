using ChessWPF.Constants;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Game
{
    public static class LegalMoveFinder
    {
        public static Dictionary<string, List<Cell>> GetLegalMovesAndProtectedCells(Piece piece,
            string fenAnnotation,
            PieceColor turnColor,
            Cell[,] cells,
            Dictionary<PieceColor, List<Piece>> pieces)
        {
            var legalMovesAndProtectedSquares = new Dictionary<string, List<Cell>>();
            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    legalMovesAndProtectedSquares = GetPawnLegalMovesAndProtectedCells(piece, turnColor, fenAnnotation, cells);
                    break;
                case PieceType.Bishop:
                    legalMovesAndProtectedSquares = GetBishopLegalMovesAndProtectedCells(piece, cells);
                    break;
                case PieceType.Knight:
                    legalMovesAndProtectedSquares = GetKnightLegalMovesAndProtectedCells(piece, cells);
                    break;
                case PieceType.Rook:
                    legalMovesAndProtectedSquares = GetRookLegalMovesAndProtectedCells(piece, cells);
                    break;
                case PieceType.Queen:
                    legalMovesAndProtectedSquares = GetQueenLegalMovesAndProtectedCells(piece, cells);
                    break;
                case PieceType.Knook:
                    legalMovesAndProtectedSquares = GetKnookLegalMovesAndProtectedCells(piece, cells);
                    break;
                case PieceType.King:
                    legalMovesAndProtectedSquares = GetKingLegalMovesAndProtectedCells(piece,
                        fenAnnotation,
                        turnColor,
                        cells,
                        pieces);
                    break;
            }
            return legalMovesAndProtectedSquares;
        }

        public static bool HasLegalMoves(Piece piece, char[,] cells,
            CellCoordinates? enPassantCoordinates, List<CellCoordinates>? pinningPieceInterceptingCells)
        {
            var turnColor = piece.Color;
            var hasLegalMoves = false;
            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    hasLegalMoves = DoesPawnHaveLegalMoves(piece.Row, piece.Col, turnColor, cells, enPassantCoordinates, pinningPieceInterceptingCells);
                    break;
                case PieceType.Knight:
                    hasLegalMoves = DoesKnightHaveLegalMoves(piece.Row, piece.Col, turnColor, cells, pinningPieceInterceptingCells);
                    break;
                case PieceType.Bishop:
                    hasLegalMoves = DoesBishopHaveLegalMoves(piece.Row, piece.Col, turnColor, cells, pinningPieceInterceptingCells);
                    break;
                case PieceType.Rook:
                    hasLegalMoves = DoesRookHaveLegalMoves(piece.Row, piece.Col, turnColor, cells, pinningPieceInterceptingCells);
                    break;
                case PieceType.Queen:
                    hasLegalMoves = DoesQueenHaveLegalMoves(piece.Row, piece.Col, turnColor, cells, pinningPieceInterceptingCells);
                    break;
                case PieceType.Knook:
                    hasLegalMoves = DoesKnookHaveLegalMoves(piece.Row, piece.Col, turnColor, cells, pinningPieceInterceptingCells);
                    break;
            }
            return hasLegalMoves;
        }

        private static bool DoesPawnHaveLegalMoves(int row,
            int col,
            PieceColor turnColor,
            char[,] cells,
            CellCoordinates? enPassantCoordinates,
            List<CellCoordinates>? interceptingCells)
        {
            if (interceptingCells != null)
            {
                if (turnColor == PieceColor.White)
                {
                    if (cells[row - 1, col] == '.'
                        && interceptingCells.Any(im => im.HasEqualCoordinates(row - 1, col)))
                    {
                        return true;
                    }
                    if (row == 6 && cells[row - 2, col] == '.'
                        && interceptingCells.Any(im => im.HasEqualCoordinates(row - 2, col)))
                    {
                        return true;
                    }
                    if (IsCellValid(row - 1, col - 1, cells))
                    {
                        if ((char.IsLower(cells[row - 1, col - 1]) ||
                           (enPassantCoordinates.HasValue
                           && enPassantCoordinates.Value.HasEqualCoordinates(row - 1, col - 1)))
                           && interceptingCells.Any(im => im.HasEqualCoordinates(row - 1, col - 1)))
                        {
                            return true;
                        }
                    }

                    if (IsCellValid(row - 1, col + 1, cells))
                    {
                        if ((char.IsLower(cells[row - 1, col + 1]) ||
                            (enPassantCoordinates.HasValue
                            && enPassantCoordinates.Value.HasEqualCoordinates(row - 1, col + 1)))
                            && interceptingCells.Any(im => im.HasEqualCoordinates(row - 1, col + 1)))
                        {
                            return true;
                        }
                    }
                }

                else if (turnColor == PieceColor.Black)
                {
                    if (cells[row + 1, col] == '.'
                        && interceptingCells.Any(im => im.HasEqualCoordinates(row + 1, col)))
                    {
                        return true;
                    }
                    if (row == 1 && cells[row + 2, col] == '.'
                        && interceptingCells.Any(im => im.HasEqualCoordinates(row + 2, col)))
                    {
                        return true;
                    }

                    if (IsCellValid(row + 1, col - 1, cells))
                    {
                        if ((char.IsUpper(cells[row + 1, col - 1]) ||
                            (enPassantCoordinates.HasValue
                            && enPassantCoordinates.Value.HasEqualCoordinates(row + 1, col - 1)))
                            && interceptingCells.Any(im => im.HasEqualCoordinates(row + 1, col - 1)))
                        {
                            return true;
                        }
                    }
                    if (IsCellValid(row + 1, col + 1, cells))
                    {
                        if (char.IsUpper(cells[row + 1, col + 1]) ||
                            (enPassantCoordinates.HasValue
                            && enPassantCoordinates.Value.HasEqualCoordinates(row + 1, col + 1))
                            && interceptingCells.Any(im => im.HasEqualCoordinates(row + 1, col + 1)))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (turnColor == PieceColor.White)
                {
                    if (cells[row - 1, col] == '.')
                    {
                        return true;
                    }
                    if (IsCellValid(row - 1, col - 1, cells))
                    {
                        if (char.IsLower(cells[row - 1, col - 1]) ||
                           (enPassantCoordinates.HasValue
                           && enPassantCoordinates.Value.HasEqualCoordinates(row - 1, col - 1)))
                        {
                            return true;
                        }
                    }

                    if (IsCellValid(row - 1, col + 1, cells))
                    {
                        if (char.IsLower(cells[row - 1, col + 1]) ||
                            (enPassantCoordinates.HasValue
                            && enPassantCoordinates.Value.HasEqualCoordinates(row - 1, col + 1)))
                        {
                            return true;
                        }
                    }
                }

                else if (turnColor == PieceColor.Black)
                {
                    if (cells[row + 1, col] == '.')
                    {
                        return true;
                    }
                    if (IsCellValid(row + 1, col - 1, cells))
                    {
                        if (char.IsUpper(cells[row + 1, col - 1]) ||
                            (enPassantCoordinates.HasValue
                            && enPassantCoordinates.Value.HasEqualCoordinates(row + 1, col - 1)))
                        {
                            return true;
                        }
                    }
                    if (IsCellValid(row + 1, col + 1, cells))
                    {
                        if (char.IsUpper(cells[row + 1, col + 1]) ||
                            (enPassantCoordinates.HasValue
                            && enPassantCoordinates.Value.HasEqualCoordinates(row + 1, col + 1)))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static Dictionary<string, List<Cell>> GetPawnLegalMovesAndProtectedCells(Piece pawn,
            PieceColor turnColor,
            string fenAnnotation,
            Cell[,] cells)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            var enPassantFromFenAnnotation = fenAnnotation.Split(" ", StringSplitOptions.RemoveEmptyEntries)[3];
            if (pawn.HasEqualCoordinates(3, 4))
            {

            }
            var enPassantMove = enPassantFromFenAnnotation != "-" ? GetCellFromAnnotation(cells, enPassantFromFenAnnotation) : null;

            if (pawn.Color == PieceColor.White)
            {
                if (turnColor == pawn.Color)
                {
                    if (cells[pawn.Row - 1, pawn.Col].Piece == null)
                    {
                        legalMoves.Add(cells[pawn.Row - 1, pawn.Col]);
                        if (pawn.Row == 6 && cells[pawn.Row - 2, pawn.Col].Piece == null)
                        {
                            legalMoves.Add(cells[pawn.Row - 2, pawn.Col]);
                        }
                    }
                }
                if (IsCellValid(pawn.Row - 1, pawn.Col - 1, cells))
                {
                    protectedCells.Add(cells[pawn.Row - 1, pawn.Col - 1]);
                    if (cells[pawn.Row - 1, pawn.Col - 1].Piece != null)
                    {
                        if (cells[pawn.Row - 1, pawn.Col - 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(cells[pawn.Row - 1, pawn.Col - 1]);
                        }
                    }
                }

                if (IsCellValid(pawn.Row - 1, pawn.Col + 1, cells))
                {
                    protectedCells.Add(cells[pawn.Row - 1, pawn.Col + 1]);
                    if (cells[pawn.Row - 1, pawn.Col + 1].Piece != null)
                    {
                        if (cells[pawn.Row - 1, pawn.Col + 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(cells[pawn.Row - 1, pawn.Col + 1]);
                        }
                    }
                }
                if (enPassantMove != null)
                {
                    var colorWhichCanTakeEnPassant = enPassantMove.Row == 2 ? PieceColor.White : PieceColor.Black;
                    if (pawn.Color == colorWhichCanTakeEnPassant && protectedCells.Any(c => c.HasEqualRowAndCol(enPassantMove)))
                    {
                        legalMoves.Add(cells[enPassantMove.Row, enPassantMove.Col]);
                    }
                }
            }
            else if (pawn.Color == PieceColor.Black)
            {
                if (turnColor == pawn.Color)
                {
                    if (cells[pawn.Row + 1, pawn.Col].Piece == null)
                    {
                        legalMoves.Add(cells[pawn.Row + 1, pawn.Col]);
                        if (pawn.Row == 1 && cells[pawn.Row + 2, pawn.Col].Piece == null)
                        {
                            legalMoves.Add(cells[pawn.Row + 2, pawn.Col]);
                        }
                    }
                }
                if (IsCellValid(pawn.Row + 1, pawn.Col - 1, cells))
                {
                    protectedCells.Add(cells[pawn.Row + 1, pawn.Col - 1]);
                    if (cells[pawn.Row + 1, pawn.Col - 1].Piece != null)
                    {
                        if (cells[pawn.Row + 1, pawn.Col - 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(cells[pawn.Row + 1, pawn.Col - 1]);
                        }
                    }
                }

                if (IsCellValid(pawn.Row + 1, pawn.Col + 1, cells))
                {
                    protectedCells.Add(cells[pawn.Row + 1, pawn.Col + 1]);
                    if (cells[pawn.Row + 1, pawn.Col + 1].Piece != null)
                    {
                        if (cells[pawn.Row + 1, pawn.Col + 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(cells[pawn.Row + 1, pawn.Col + 1]);
                        }
                    }
                }
                if (enPassantMove != null)
                {
                    var colorWhichCanTakeEnPassant = enPassantMove.Row == 2 ? PieceColor.White : PieceColor.Black;
                    if (pawn.Color == colorWhichCanTakeEnPassant && protectedCells.Any(c => c.HasEqualRowAndCol(enPassantMove)))
                    {
                        legalMoves.Add(cells[enPassantMove.Row, enPassantMove.Col]);
                    }
                }
            }
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static bool DoesBishopHaveLegalMoves(int row, int col, PieceColor turnColor, char[,] cells, List<CellCoordinates>? interceptingCells)
        {
            Func<int, int, bool> isValidMove = (rowParam, colParam) => (IsCellValid(rowParam, colParam, cells) && (cells[rowParam, colParam] == '.'
             || (turnColor == PieceColor.White ? char.IsLower(cells[rowParam, colParam]) : char.IsUpper(cells[rowParam, colParam])))
             && (interceptingCells != null ? (interceptingCells.Any(im => im.HasEqualCoordinates(rowParam, colParam)) ? true : false) : true));
            int increment = 1;

            while (IsCellValid(row + increment, col + increment, cells))
            {
                if (isValidMove(row + increment, col + increment))
                {
                    return true;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(row + increment, col - increment, cells))
            {
                if (isValidMove(row + increment, col - increment))
                {
                    return true;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(row - increment, col - increment, cells))
            {
                if (isValidMove(row - increment, col - increment))
                {
                    return true;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(row - increment, col + increment, cells))
            {
                if (isValidMove(row - increment, col + increment))
                {
                    return true;
                }
                increment++;
            }
            return false;
        }

        private static Dictionary<string, List<Cell>> GetBishopLegalMovesAndProtectedCells(Piece bishop, Cell[,] cells)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();

            var increment = 1;

            while (IsCellValid(bishop.Row + increment, bishop.Col + increment, cells))
            {
                var currCell = cells[bishop.Row + increment, bishop.Col + increment];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != bishop.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(bishop.Row + ++increment, bishop.Col + increment, cells))
                            {
                                currCell = cells[bishop.Row + increment, bishop.Col + increment];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == bishop.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(bishop.Row + increment, bishop.Col - increment, cells))
            {
                var currCell = cells[bishop.Row + increment, bishop.Col - increment];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != bishop.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(bishop.Row + ++increment, bishop.Col - increment, cells))
                            {
                                currCell = cells[bishop.Row + increment, bishop.Col - increment];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == bishop.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(bishop.Row - increment, bishop.Col - increment, cells))
            {
                var currCell = cells[bishop.Row - increment, bishop.Col - increment];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != bishop.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(bishop.Row - ++increment, bishop.Col - increment, cells))
                            {
                                currCell = cells[bishop.Row - increment, bishop.Col - increment];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == bishop.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(bishop.Row - increment, bishop.Col + increment, cells))
            {
                var currCell = cells[bishop.Row - increment, bishop.Col + increment];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != bishop.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(bishop.Row - ++increment, bishop.Col + increment, cells))
                            {
                                currCell = cells[bishop.Row - increment, bishop.Col + increment];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == bishop.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static bool DoesKnightHaveLegalMoves(int row,
            int col,
            PieceColor turnColor,
            char[,] cells,
            List<CellCoordinates>? interceptingCells)
        {
            Func<int, int, bool> isValidMove = (rowParam, colParam) => (IsCellValid(rowParam, colParam, cells) && (cells[rowParam, colParam] == '.'
            || (turnColor == PieceColor.White ? char.IsLower(cells[rowParam, colParam]) : char.IsUpper(cells[rowParam, colParam])))
            && (interceptingCells != null ? (interceptingCells.Any(im => im.HasEqualCoordinates(rowParam, colParam)) ? true : false) : true));
            if (isValidMove(row - 2, col - 1))
            {
                return true;
            }
            if (isValidMove(row - 2, col + 1))
            {
                return true;
            }
            if (isValidMove(row + 2, col - 1))
            {
                return true;
            }
            if (isValidMove(row + 2, col + 1))
            {
                return true;
            }
            if (isValidMove(row - 1, col - 2))
            {
                return true;
            }
            if (isValidMove(row + 1, col - 2))
            {
                return true;
            }
            if (isValidMove(row - 1, col + 2))
            {
                return true;
            }
            if (isValidMove(row + 1, col + 2))
            {
                return true;
            }
            return false;
        }

        private static Dictionary<string, List<Cell>> GetKnightLegalMovesAndProtectedCells(Piece knight, Cell[,] cells)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();

            var pieceCell = cells[knight.Row, knight.Col];
            var currCell = pieceCell;
            if (IsCellValid(pieceCell.Row - 2, pieceCell.Col - 1, cells))
            {
                currCell = cells[pieceCell.Row - 2, pieceCell.Col - 1];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }

            }
            if (IsCellValid(pieceCell.Row - 2, pieceCell.Col + 1, cells))
            {
                currCell = cells[pieceCell.Row - 2, pieceCell.Col + 1];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }
            if (IsCellValid(pieceCell.Row + 2, pieceCell.Col - 1, cells))
            {
                currCell = cells[pieceCell.Row + 2, pieceCell.Col - 1];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }
            if (IsCellValid(pieceCell.Row + 2, pieceCell.Col + 1, cells))
            {
                currCell = cells[pieceCell.Row + 2, pieceCell.Col + 1];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }

            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col - 2, cells))
            {
                currCell = cells[pieceCell.Row - 1, pieceCell.Col - 2];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col - 2, cells))
            {
                currCell = cells[pieceCell.Row + 1, pieceCell.Col - 2];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }

            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col + 2, cells))
            {
                currCell = cells[pieceCell.Row - 1, pieceCell.Col + 2];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col + 2, cells))
            {
                currCell = cells[pieceCell.Row + 1, pieceCell.Col + 2];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece.Color != knight.Color)
                {
                    legalMoves.Add(currCell);
                }
                else
                {
                    protectedCells.Add(currCell);
                }
            }
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static bool DoesRookHaveLegalMoves(int row,
            int col,
            PieceColor turnColor,
            char[,] cells,
            List<CellCoordinates>? interceptingCells)
        {
            Func<int, int, bool> isValidMove = (rowParam, colParam) => (IsCellValid(rowParam, colParam, cells) && (cells[rowParam, colParam] == '.'
             || (turnColor == PieceColor.White ? char.IsLower(cells[rowParam, colParam]) : char.IsUpper(cells[rowParam, colParam])))
             && (interceptingCells != null ? (interceptingCells.Any(im => im.HasEqualCoordinates(rowParam, colParam)) ? true : false) : true));
            var increment = 1;
            while (IsCellValid(row + increment, col, cells))
            {
                if (isValidMove(row + increment, col))
                {
                    return true;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(row - increment, col, cells))
            {
                if (isValidMove(row - increment, col))
                {
                    return true;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(row, col + increment, cells))
            {
                if (isValidMove(row, col + increment))
                {
                    return true;
                }
                increment++;
            }
            increment = 1;

            while (IsCellValid(row, col - increment, cells))
            {
                if (isValidMove(row, col - increment))
                {
                    return true;
                }
                increment++;
            }
            return false;
        }

        private static Dictionary<string, List<Cell>> GetRookLegalMovesAndProtectedCells(Piece rook, Cell[,] cells)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            int increment = 1;

            while (IsCellValid(rook.Row - increment, rook.Col, cells))
            {
                var currCell = cells[rook.Row - increment, rook.Col];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != rook.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(rook.Row - ++increment, rook.Col, cells))
                            {
                                currCell = cells[rook.Row - increment, rook.Col];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == rook.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(rook.Row + increment, rook.Col, cells))
            {
                var currCell = cells[rook.Row + increment, rook.Col];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != rook.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(rook.Row + ++increment, rook.Col, cells))
                            {
                                currCell = cells[rook.Row + increment, rook.Col];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == rook.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(rook.Row, rook.Col - increment, cells))
            {
                var currCell = cells[rook.Row, rook.Col - increment];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != rook.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(rook.Row, rook.Col - ++increment, cells))
                            {
                                currCell = cells[rook.Row, rook.Col - increment];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == rook.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            while (IsCellValid(rook.Row, rook.Col + increment, cells))
            {
                var currCell = cells[rook.Row, rook.Col + increment];
                if (currCell.Piece == null)
                {
                    legalMoves.Add(currCell);
                }
                else if (currCell.Piece != null)
                {
                    if (currCell.Piece.Color != rook.Color)
                    {
                        legalMoves.Add(currCell);
                        if (currCell.Piece.PieceType == PieceType.King)
                        {
                            while (IsCellValid(rook.Row, rook.Col + ++increment, cells))
                            {
                                currCell = cells[rook.Row, rook.Col + increment];
                                if (currCell.Piece != null)
                                {
                                    if (currCell.Piece.Color == rook.Color)
                                    {
                                        protectedCells.Add(currCell);
                                    }
                                    break;
                                }
                                protectedCells.Add(currCell);
                            }
                        }
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                    break;
                }
                increment++;
            }
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static bool DoesQueenHaveLegalMoves(int row, int col, PieceColor turnColor, char[,] cells, List<CellCoordinates>? interceptingCells)
        {
            return (DoesBishopHaveLegalMoves(row, col, turnColor, cells, interceptingCells) || DoesRookHaveLegalMoves(row, col, turnColor, cells, interceptingCells));
        }

        private static Dictionary<string, List<Cell>> GetQueenLegalMovesAndProtectedCells(Piece queen, Cell[,] cells)
        {
            var bishopLegalMovesAndProtectedCells = GetBishopLegalMovesAndProtectedCells(queen, cells);
            var rookLegalMovesAndProtectedCells = GetRookLegalMovesAndProtectedCells(queen, cells);
            var queenLegalMoves = new Dictionary<string, List<Cell>>
            {
                { LegalMovesAndProtectedCells.LegalMoves, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Union(bishopLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]).ToList() },
                { LegalMovesAndProtectedCells.ProtectedCells, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells].Union(bishopLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells]).ToList() }
            };

            return queenLegalMoves;
        }

        private static bool DoesKnookHaveLegalMoves(int row, int col, PieceColor turnColor, char[,] cells, List<CellCoordinates>? interceptingCells)
        {
            return (DoesKnightHaveLegalMoves(row, col, turnColor, cells, interceptingCells) || DoesRookHaveLegalMoves(row, col, turnColor, cells, interceptingCells));
        }

        private static Dictionary<string, List<Cell>> GetKnookLegalMovesAndProtectedCells(Piece knook, Cell[,] cells)
        {
            var rookLegalMovesAndProtectedCells = GetRookLegalMovesAndProtectedCells(knook, cells);
            var knightLegalMoves = GetKnightLegalMovesAndProtectedCells(knook, cells);
            var knookLegalMoves = new Dictionary<string, List<Cell>>
            {
                { LegalMovesAndProtectedCells.LegalMoves, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Union(knightLegalMoves[LegalMovesAndProtectedCells.LegalMoves]).ToList() },
                { LegalMovesAndProtectedCells.ProtectedCells, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells].Union(knightLegalMoves[LegalMovesAndProtectedCells.ProtectedCells]).ToList() }
            };

            return knookLegalMoves;
        }

        private static Dictionary<string, List<Cell>> GetKingLegalMovesAndProtectedCells(Piece king,
            string fenAnnotation,
            PieceColor turnColor,
            Cell[,] cells,
            Dictionary<PieceColor, List<Piece>> pieces)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            var pieceCell = cells[king.Row, king.Col];
            var currCell = cells[king.Row, king.Col];
            var oppositeColor = king.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col, cells))
            {
                currCell = cells[pieceCell.Row - 1, pieceCell.Col];
                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col - 1, cells))
            {
                currCell = cells[pieceCell.Row - 1, pieceCell.Col - 1];

                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col + 1, cells))
            {
                currCell = cells[pieceCell.Row - 1, pieceCell.Col + 1];

                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row, pieceCell.Col - 1, cells))
            {
                currCell = cells[pieceCell.Row, pieceCell.Col - 1];
                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row, pieceCell.Col + 1, cells))
            {
                currCell = cells[pieceCell.Row, pieceCell.Col + 1];
                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }

            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col, cells))
            {
                currCell = cells[pieceCell.Row + 1, pieceCell.Col];
                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col - 1, cells))
            {
                currCell = cells[pieceCell.Row + 1, pieceCell.Col - 1];
                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col + 1, cells))
            {
                currCell = cells[pieceCell.Row + 1, pieceCell.Col + 1];
                if (currCell.Piece == null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
                else if (currCell.Piece != null)
                {
                    if ((!pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (turnColor == king.Color)
            {
                var fenCastlingRights = fenAnnotation.Split(" ", StringSplitOptions.RemoveEmptyEntries)[2];
                var castlingRights = FenAnnotationReader.GetCastlingRights(fenCastlingRights);
                var kingCastlingRights = king.Color == PieceColor.White ? castlingRights.Take(2) : castlingRights
                    .Skip(2)
                    .Take(2);
                    
                if (kingCastlingRights.Any(kr => kr) &&
                    !pieces[oppositeColor].Any(p => p.LegalMoves.Any(lm => lm.Row == king.Row && lm.Col == king.Col))
                    && king.Col == 4)
                {
                    var legalCastlingMoves = CheckForCastling(king, oppositeColor, kingCastlingRights.ToArray(), cells, pieces);
                    if (legalCastlingMoves.Count > 0)
                    {
                        legalMoves.AddRange(legalCastlingMoves);
                    }
                }
            }

            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static List<Cell> CheckForCastling(Piece king,
            PieceColor oppositeColor,
            bool[] castlingRights,
            Cell[,] cells,
            Dictionary<PieceColor, List<Piece>> pieces
            )
        {
            var legalCastlingMoves = new List<Cell>();

            if (castlingRights[0] && cells[king.Row, king.Col + 3].Piece != null
                && cells[king.Row, king.Col + 3].Piece!.PieceType == PieceType.Rook)
            {
                var rook = cells[king.Row, king.Col + 3].Piece;
                bool wayIsClear = true;
                for (int i = king.Col + 1; i < cells.GetLength(1) - 1; i++)
                {
                    if (cells[king.Row, i].Piece != null)
                    {
                        wayIsClear = false;
                        break;
                    }
                }

                if (wayIsClear)
                {
                    if (!pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col + 2))
                    && !pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col + 1))
                    && !pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col + 2))
                    && !pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col + 1)))
                    {
                        legalCastlingMoves.Add(cells[king.Row, king.Col + 2]);
                    }
                }
            }

            if (castlingRights[1] && cells[king.Row, king.Col - 4].Piece != null
                && cells[king.Row, king.Col - 4].Piece!.PieceType == PieceType.Rook)
            {
                var rook = cells[king.Row, king.Col - 4].Piece;

                bool wayIsClear = true;
                for (int i = king.Col - 1; i > 1; i--)
                {
                    if (cells[king.Row, i].Piece != null)
                    {
                        wayIsClear = false;
                        break;
                    }
                }
                if (wayIsClear)
                {
                    if (!pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col - 2))
                    && !pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col - 1))
                    && !pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col - 2))
                    && !pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col - 1)))
                    {
                        legalCastlingMoves.Add(cells[king.Row, king.Col - 2]);
                    }
                }
            }
            return legalCastlingMoves;
        }

        private static bool IsCellValid(int row, int col, Cell[,] cells)
        {
            return row >= 0 && row < cells.GetLength(0) && col >= 0 && col < cells.GetLength(1);
        }

        private static bool IsCellValid(int row, int col, char[,] cells)
        {
            return row >= 0 && row < cells.GetLength(0) && col >= 0 && col < cells.GetLength(1);
        }

        private static Cell GetCellFromAnnotation(Cell[,] cells, string annotation)
        {
            return cells[8 - (int)(annotation[1] - 48), (int)(annotation[0] - 97)];
        }
    }
}
