using ChessWPF.Constants;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Singleton;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ChessWPF.Game
{
    public static class LegalMoveFinder
    {
        public static Dictionary<string, List<Cell>> GetLegalMovesAndProtectedCells(Piece piece)
        {
            var board = BackgroundSingleton.Instance.Board;
            var legalMovesAndProtectedSquares = new Dictionary<string, List<Cell>>();
            if (piece.PieceType == PieceType.Pawn)
            {
                legalMovesAndProtectedSquares = GetPawnLegalMovesAndProtectedCells(piece, board);
            }
            else if (piece.PieceType == PieceType.Bishop)
            {
                legalMovesAndProtectedSquares = GetBishopLegalMovesAndProtectedCells(piece, board);
            }
            else if (piece.PieceType == PieceType.Knight)
            {
                legalMovesAndProtectedSquares = GetKnightLegalMovesAndProtectedCells(piece, board);
            }
            else if (piece.PieceType == PieceType.Rook)
            {
                legalMovesAndProtectedSquares = GetRookLegalMovesAndProtectedCells(piece, board);
            }
            else if (piece.PieceType == PieceType.Queen)
            {
                legalMovesAndProtectedSquares = GetQueenLegalMovesAndProtectedCells(piece, board);
            }
            else if (piece.PieceType == PieceType.King)
            {
                legalMovesAndProtectedSquares = GetKingLegalMovesAndProtectedCells(piece, board);
            }
            return legalMovesAndProtectedSquares;
        }

        private static Dictionary<string, List<Cell>> GetPawnLegalMovesAndProtectedCells(Piece pawn, Board board)
        {
            var turnColor = board.Moves.Count % 2 == 0 ? PieceColor.White : PieceColor.Black;
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            var lastMove = board.Moves.Count > 0
                && board.Moves.Peek().CellOneBefore.Piece.PieceType == PieceType.Pawn
                && board.Moves.Peek().CellTwoAfter.Row == pawn.Cell.Row
                && Math.Abs(board.Moves.Peek().CellOneBefore.Row - board.Moves.Peek().CellTwoBefore.Row) == 2
                && Math.Abs(board.Moves.Peek().CellOneBefore.Col - pawn.Cell.Col) == 1
                ? board.Moves.Peek().CellTwoAfter.Piece
                : null;


            if (pawn.Color == PieceColor.White)
            {
                if (turnColor == pawn.Color)
                {
                    if (board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col].Piece == null)
                    {
                        legalMoves.Add(board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col]);
                        if (pawn.Cell.Row == 6 && board.Cells[pawn.Cell.Row - 2, pawn.Cell.Col].Piece == null)
                        {
                            legalMoves.Add(board.Cells[pawn.Cell.Row - 2, pawn.Cell.Col]);
                        }
                    }
                }
                if (IsCellValid(pawn.Cell.Row - 1, pawn.Cell.Col - 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col - 1]);
                    if (board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col - 1].Piece != null)
                    {
                        if (board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col - 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col - 1]);
                        }
                    }
                }

                if (IsCellValid(pawn.Cell.Row - 1, pawn.Cell.Col + 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col + 1]);
                    if (board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col + 1].Piece != null)
                    {
                        if (board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col + 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Cell.Row - 1, pawn.Cell.Col + 1]);
                        }
                    }
                }
                if (lastMove != null)
                {
                    legalMoves.Add(board.Cells[lastMove.Cell.Row - 1, lastMove.Cell.Col]);
                }
            }
            else if (pawn.Color == PieceColor.Black)
            {
                if (turnColor == pawn.Color)
                {
                    if (board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col].Piece == null)
                    {
                        legalMoves.Add(board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col]);
                        if (pawn.Cell.Row == 1 && board.Cells[pawn.Cell.Row + 2, pawn.Cell.Col].Piece == null)
                        {
                            legalMoves.Add(board.Cells[pawn.Cell.Row + 2, pawn.Cell.Col]);
                        }
                    }
                }
                if (IsCellValid(pawn.Cell.Row + 1, pawn.Cell.Col - 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col - 1]);
                    if (board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col - 1].Piece != null)
                    {
                        if (board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col - 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col - 1]);
                        }
                    }
                }

                if (IsCellValid(pawn.Cell.Row + 1, pawn.Cell.Col + 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col + 1]);
                    if (board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col + 1].Piece != null)
                    {
                        if (board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col + 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Cell.Row + 1, pawn.Cell.Col + 1]);
                        }
                    }
                }
                if (lastMove != null)
                {
                    legalMoves.Add(board.Cells[lastMove.Cell.Row + 1, lastMove.Cell.Col]);
                }
            }
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static Dictionary<string, List<Cell>> GetBishopLegalMovesAndProtectedCells(Piece bishop, Board board)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();

            int rowIncrement = 1;
            int colIncrement = 1;

            while (IsCellValid(bishop.Cell.Row + rowIncrement, bishop.Cell.Col + colIncrement, board))
            {
                var currCell = board.Cells[bishop.Cell.Row + rowIncrement, bishop.Cell.Col + colIncrement];
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
                            while (IsCellValid(bishop.Cell.Row + ++rowIncrement, bishop.Cell.Col + ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Cell.Row + rowIncrement, bishop.Cell.Col + colIncrement];
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
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (IsCellValid(bishop.Cell.Row + rowIncrement, bishop.Cell.Col - colIncrement, board))
            {
                var currCell = board.Cells[bishop.Cell.Row + rowIncrement, bishop.Cell.Col - colIncrement];
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
                            while (IsCellValid(bishop.Cell.Row + ++rowIncrement, bishop.Cell.Col - ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Cell.Row + rowIncrement, bishop.Cell.Col - colIncrement];
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
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (IsCellValid(bishop.Cell.Row - rowIncrement, bishop.Cell.Col - colIncrement, board))
            {
                var currCell = board.Cells[bishop.Cell.Row - rowIncrement, bishop.Cell.Col - colIncrement];
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
                            while (IsCellValid(bishop.Cell.Row - ++rowIncrement, bishop.Cell.Col - ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Cell.Row - rowIncrement, bishop.Cell.Col - colIncrement];
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
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (IsCellValid(bishop.Cell.Row - rowIncrement, bishop.Cell.Col + colIncrement, board))
            {
                var currCell = board.Cells[bishop.Cell.Row - rowIncrement, bishop.Cell.Col + colIncrement];
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
                            while (IsCellValid(bishop.Cell.Row - ++rowIncrement, bishop.Cell.Col + ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Cell.Row - rowIncrement, bishop.Cell.Col + colIncrement];
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
                rowIncrement++;
                colIncrement++;
            }
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static Dictionary<string, List<Cell>> GetKnightLegalMovesAndProtectedCells(Piece knight, Board board)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();

            var pieceCell = knight.Cell;
            var currCell = pieceCell;
            if (IsCellValid(pieceCell.Row - 2, pieceCell.Col - 1, board))
            {
                currCell = board.Cells[pieceCell.Row - 2, pieceCell.Col - 1];
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
            if (IsCellValid(pieceCell.Row - 2, pieceCell.Col + 1, board))
            {
                currCell = board.Cells[pieceCell.Row - 2, pieceCell.Col + 1];
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
            if (IsCellValid(pieceCell.Row + 2, pieceCell.Col - 1, board))
            {
                currCell = board.Cells[pieceCell.Row + 2, pieceCell.Col - 1];
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
            if (IsCellValid(pieceCell.Row + 2, pieceCell.Col + 1, board))
            {
                currCell = board.Cells[pieceCell.Row + 2, pieceCell.Col + 1];
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

            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col - 2, board))
            {
                currCell = board.Cells[pieceCell.Row - 1, pieceCell.Col - 2];
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
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col - 2, board))
            {
                currCell = board.Cells[pieceCell.Row + 1, pieceCell.Col - 2];
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

            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col + 2, board))
            {
                currCell = board.Cells[pieceCell.Row - 1, pieceCell.Col + 2];
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
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col + 2, board))
            {
                currCell = board.Cells[pieceCell.Row + 1, pieceCell.Col + 2];
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

        private static Dictionary<string, List<Cell>> GetRookLegalMovesAndProtectedCells(Piece rook, Board board)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            int increment = 1;

            while (IsCellValid(rook.Cell.Row - increment, rook.Cell.Col, board))
            {
                var currCell = board.Cells[rook.Cell.Row - increment, rook.Cell.Col];
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
                            while (IsCellValid(rook.Cell.Row - ++increment, rook.Cell.Col, board))
                            {
                                currCell = board.Cells[rook.Cell.Row - increment, rook.Cell.Col];
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
            while (IsCellValid(rook.Cell.Row + increment, rook.Cell.Col, board))
            {
                var currCell = board.Cells[rook.Cell.Row + increment, rook.Cell.Col];
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
                            while (IsCellValid(rook.Cell.Row + ++increment, rook.Cell.Col, board))
                            {
                                currCell = board.Cells[rook.Cell.Row + increment, rook.Cell.Col];
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
            while (IsCellValid(rook.Cell.Row, rook.Cell.Col - increment, board))
            {
                var currCell = board.Cells[rook.Cell.Row, rook.Cell.Col - increment];
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
                            while (IsCellValid(rook.Cell.Row, rook.Cell.Col - ++increment, board))
                            {
                                currCell = board.Cells[rook.Cell.Row, rook.Cell.Col - increment];
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
            while (IsCellValid(rook.Cell.Row, rook.Cell.Col + increment, board))
            {
                var currCell = board.Cells[rook.Cell.Row, rook.Cell.Col + increment];
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
                            while (IsCellValid(rook.Cell.Row, rook.Cell.Col + ++increment, board))
                            {
                                currCell = board.Cells[rook.Cell.Row, rook.Cell.Col + increment];
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

        private static Dictionary<string, List<Cell>> GetQueenLegalMovesAndProtectedCells(Piece queen, Board board)
        {
            var bishopLegalMovesAndProtectedCells = GetBishopLegalMovesAndProtectedCells(queen, board);
            var rookLegalMovesAndProtectedCells = GetRookLegalMovesAndProtectedCells(queen, board);
            var queenLegalMoves = new Dictionary<string, List<Cell>>();
            queenLegalMoves.Add(LegalMovesAndProtectedCells.LegalMoves, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Union(bishopLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]).ToList());
            queenLegalMoves.Add(LegalMovesAndProtectedCells.ProtectedCells, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells].Union(bishopLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells]).ToList());


            return queenLegalMoves;
        }

        private static Dictionary<string, List<Cell>> GetKingLegalMovesAndProtectedCells(Piece king, Board board)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            var pieceCell = king.Cell;
            var currCell = king.Cell;
            var oppositeColor = king.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col, board))
            {
                currCell = board.Cells[pieceCell.Row - 1, pieceCell.Col];
                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col - 1, board))
            {
                currCell = board.Cells[pieceCell.Row - 1, pieceCell.Col - 1];

                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row - 1, pieceCell.Col + 1, board))
            {
                currCell = board.Cells[pieceCell.Row - 1, pieceCell.Col + 1];

                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row, pieceCell.Col - 1, board))
            {
                currCell = board.Cells[pieceCell.Row, pieceCell.Col - 1];
                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row, pieceCell.Col + 1, board))
            {
                currCell = board.Cells[pieceCell.Row, pieceCell.Col + 1];
                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }

            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col, board))
            {
                currCell = board.Cells[pieceCell.Row + 1, pieceCell.Col];
                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col - 1, board))
            {
                currCell = board.Cells[pieceCell.Row + 1, pieceCell.Col - 1];
                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }
            if (IsCellValid(pieceCell.Row + 1, pieceCell.Col + 1, board))
            {
                currCell = board.Cells[pieceCell.Row + 1, pieceCell.Col + 1];
                if (currCell.Piece == null)
                {
                    if ((!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Contains(currCell))) && (!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell))))
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
                    if ((!board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Contains(currCell)) && currCell.Piece.Color != king.Color))
                    {
                        legalMoves.Add(currCell);
                    }
                    else
                    {
                        protectedCells.Add(currCell);
                    }
                }
            }

            if (!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(lm => lm.Row == king.Cell.Row && lm.Col == king.Cell.Col))
                && !board.Moves.Any(m => m.CellTwoAfter.Piece.PieceType == PieceType.King && m.CellTwoAfter.Piece.Color == king.Color))
            {
                var legalCastlingMoves = CheckForCastling(king, board, oppositeColor);
                if (legalCastlingMoves.Count > 0)
                {
                    legalMoves.AddRange(legalCastlingMoves);
                }
            }

            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.LegalMoves, legalMoves);
            legalMovesAndProtectedCells.Add(LegalMovesAndProtectedCells.ProtectedCells, protectedCells);
            return legalMovesAndProtectedCells;
        }

        private static List<Cell> CheckForCastling(Piece king, Board board, PieceColor oppositeColor)
        {
            var legalCastlingMoves = new List<Cell>();
            if (board.Cells[king.Cell.Row, king.Cell.Col + 3].Piece != null && board.Cells[king.Cell.Row, king.Cell.Col + 3].Piece.PieceType == PieceType.Rook)
            {
                var rook = board.Cells[king.Cell.Row, king.Cell.Col + 3].Piece;
                bool wayIsClear = true;
                for (int i = king.Cell.Col + 1; i < board.Cells.GetLength(1) - 1; i++)
                {
                    if (board.Cells[king.Cell.Row, i].Piece != null)
                    {
                        wayIsClear = false;
                        break;
                    }
                }

                if (!board.Moves.Any(m => m.CellOneBefore.Row == rook.Cell.Row && m.CellOneBefore.Col == rook.Cell.Col) && wayIsClear)
                {
                    if (!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col + 2))
                    && !board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col + 1))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col + 2))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col + 1)))
                    {
                        legalCastlingMoves.Add(board.Cells[king.Cell.Row, king.Cell.Col + 2]);
                    }
                }
            }

            if (board.Cells[king.Cell.Row, king.Cell.Col - 4].Piece != null && board.Cells[king.Cell.Row, king.Cell.Col - 4].Piece.PieceType == PieceType.Rook)
            {
                var rook = board.Cells[king.Cell.Row, king.Cell.Col - 4].Piece;

                bool wayIsClear = true;
                for (int i = king.Cell.Col - 1; i > 1; i--)
                {
                    if (board.Cells[king.Cell.Row, i].Piece != null)
                    {
                        wayIsClear = false;
                        break;
                    }
                }
                if (!board.Moves.Any(m => m.CellOneBefore.Row == rook.Cell.Row && m.CellOneBefore.Col == rook.Cell.Col) && wayIsClear)
                {
                    if (!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col - 2))
                    && !board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col - 1))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col - 2))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Cell.Row && m.Col == king.Cell.Col - 1)))
                    {
                        legalCastlingMoves.Add(board.Cells[king.Cell.Row, king.Cell.Col - 2]);
                    }
                }
            }
            return legalCastlingMoves;
        }

        private static bool IsCellValid(int row, int col, Board board)
        {
            return row >= 0 && row < board.Cells.GetLength(0) && col >= 0 && col < board.Cells.GetLength(1);
        }
    }
}
