using ChessWPF.Constants;
using ChessWPF.Models.Boards;
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
        public static Dictionary<string, List<Cell>> GetLegalMovesAndProtectedCells(Piece piece, Board board)
        {
            var legalMovesAndProtectedSquares = new Dictionary<string, List<Cell>>();
            switch (piece.PieceType)
            {
                case PieceType.Pawn:
                    legalMovesAndProtectedSquares = GetPawnLegalMovesAndProtectedCells(piece, board);
                    break;
                case PieceType.Bishop:
                    legalMovesAndProtectedSquares = GetBishopLegalMovesAndProtectedCells(piece, board);
                    break;
                case PieceType.Knight:
                    legalMovesAndProtectedSquares = GetKnightLegalMovesAndProtectedCells(piece, board);
                    break;
                case PieceType.Rook:
                    legalMovesAndProtectedSquares = GetRookLegalMovesAndProtectedCells(piece, board);
                    break;
                case PieceType.Queen:
                    legalMovesAndProtectedSquares = GetQueenLegalMovesAndProtectedCells(piece, board);
                    break;
                case PieceType.Knook:
                    legalMovesAndProtectedSquares = GetKnookLegalMovesAndProtectedCells(piece, board);
                    break;
                case PieceType.King:
                    legalMovesAndProtectedSquares = GetKingLegalMovesAndProtectedCells(piece, board);
                    break;
            }
            return legalMovesAndProtectedSquares;
        }

        private static Dictionary<string, List<Cell>> GetPawnLegalMovesAndProtectedCells(Piece pawn, Board board)
        {
            var turnColor = board.TurnColor;
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            var enPassantFromFenAnnotation = board.FenAnnotation.Split(" ", StringSplitOptions.RemoveEmptyEntries)[3];
            var enPassantMove = enPassantFromFenAnnotation != "-" ? GetCellFromAnnotation(board, enPassantFromFenAnnotation) : null;
            if (board.Moves.Count > 0)
            {
                if (board.Moves.Peek().CellOneBefore.Piece!.PieceType == PieceType.Pawn
                   && board.Moves.Peek().CellTwoAfter.Row == pawn.Row
                   && Math.Abs(board.Moves.Peek().CellOneBefore.Row - board.Moves.Peek().CellTwoBefore.Row) == 2
                   && Math.Abs(board.Moves.Peek().CellOneBefore.Col - pawn.Col) == 1)
                {
                    if (pawn.Color == PieceColor.White)
                    {
                        enPassantMove = board.Cells[board.Moves.Peek().CellTwoAfter.Row - 1, board.Moves.Peek().CellTwoAfter.Col];
                    }
                    else
                    {
                        enPassantMove = board.Cells[board.Moves.Peek().CellTwoAfter.Row + 1, board.Moves.Peek().CellTwoAfter.Col];
                    }
                }
            }
            if (pawn.Row == 3 && pawn.Col == 5)
            {

            }

            if (pawn.Color == PieceColor.White)
            {
                if (turnColor == pawn.Color)
                {
                    if (board.Cells[pawn.Row - 1, pawn.Col].Piece == null)
                    {
                        legalMoves.Add(board.Cells[pawn.Row - 1, pawn.Col]);
                        if (pawn.Row == 6 && board.Cells[pawn.Row - 2, pawn.Col].Piece == null)
                        {
                            legalMoves.Add(board.Cells[pawn.Row - 2, pawn.Col]);
                        }
                    }
                }
                if (IsCellValid(pawn.Row - 1, pawn.Col - 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Row - 1, pawn.Col - 1]);
                    if (board.Cells[pawn.Row - 1, pawn.Col - 1].Piece != null)
                    {
                        if (board.Cells[pawn.Row - 1, pawn.Col - 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Row - 1, pawn.Col - 1]);
                        }
                    }
                }

                if (IsCellValid(pawn.Row - 1, pawn.Col + 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Row - 1, pawn.Col + 1]);
                    if (board.Cells[pawn.Row - 1, pawn.Col + 1].Piece != null)
                    {
                        if (board.Cells[pawn.Row - 1, pawn.Col + 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Row - 1, pawn.Col + 1]);
                        }
                    }
                }
                if (enPassantMove != null)
                {
                    var colorWhichCanTakeEnPassant = enPassantMove.Row == 2 ? PieceColor.White : PieceColor.Black;
                    if (pawn.Color == colorWhichCanTakeEnPassant && protectedCells.Any(c => c.HasEqualRowAndCol(enPassantMove)))
                    {
                        legalMoves.Add(board.Cells[enPassantMove.Row, enPassantMove.Col]);
                    }
                }
            }
            else if (pawn.Color == PieceColor.Black)
            {
                if (turnColor == pawn.Color)
                {
                    if (board.Cells[pawn.Row + 1, pawn.Col].Piece == null)
                    {
                        legalMoves.Add(board.Cells[pawn.Row + 1, pawn.Col]);
                        if (pawn.Row == 1 && board.Cells[pawn.Row + 2, pawn.Col].Piece == null)
                        {
                            legalMoves.Add(board.Cells[pawn.Row + 2, pawn.Col]);
                        }
                    }
                }
                if (IsCellValid(pawn.Row + 1, pawn.Col - 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Row + 1, pawn.Col - 1]);
                    if (board.Cells[pawn.Row + 1, pawn.Col - 1].Piece != null)
                    {
                        if (board.Cells[pawn.Row + 1, pawn.Col - 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Row + 1, pawn.Col - 1]);
                        }
                    }
                }

                if (IsCellValid(pawn.Row + 1, pawn.Col + 1, board))
                {
                    protectedCells.Add(board.Cells[pawn.Row + 1, pawn.Col + 1]);
                    if (board.Cells[pawn.Row + 1, pawn.Col + 1].Piece != null)
                    {
                        if (board.Cells[pawn.Row + 1, pawn.Col + 1].Piece?.Color != pawn.Color)
                        {
                            legalMoves.Add(board.Cells[pawn.Row + 1, pawn.Col + 1]);
                        }
                    }
                }
                if (enPassantMove != null)
                {
                    var colorWhichCanTakeEnPassant = enPassantMove.Row == 2 ? PieceColor.White : PieceColor.Black;
                    if (pawn.Color == colorWhichCanTakeEnPassant && protectedCells.Any(c => c.HasEqualRowAndCol(enPassantMove)))
                    {
                        legalMoves.Add(board.Cells[enPassantMove.Row, enPassantMove.Col]);
                    }
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

            while (IsCellValid(bishop.Row + rowIncrement, bishop.Col + colIncrement, board))
            {
                var currCell = board.Cells[bishop.Row + rowIncrement, bishop.Col + colIncrement];
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
                            while (IsCellValid(bishop.Row + ++rowIncrement, bishop.Col + ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Row + rowIncrement, bishop.Col + colIncrement];
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
            while (IsCellValid(bishop.Row + rowIncrement, bishop.Col - colIncrement, board))
            {
                var currCell = board.Cells[bishop.Row + rowIncrement, bishop.Col - colIncrement];
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
                            while (IsCellValid(bishop.Row + ++rowIncrement, bishop.Col - ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Row + rowIncrement, bishop.Col - colIncrement];
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
            while (IsCellValid(bishop.Row - rowIncrement, bishop.Col - colIncrement, board))
            {
                var currCell = board.Cells[bishop.Row - rowIncrement, bishop.Col - colIncrement];
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
                            while (IsCellValid(bishop.Row - ++rowIncrement, bishop.Col - ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Row - rowIncrement, bishop.Col - colIncrement];
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
            while (IsCellValid(bishop.Row - rowIncrement, bishop.Col + colIncrement, board))
            {
                var currCell = board.Cells[bishop.Row - rowIncrement, bishop.Col + colIncrement];
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
                            while (IsCellValid(bishop.Row - ++rowIncrement, bishop.Col + ++colIncrement, board))
                            {
                                currCell = board.Cells[bishop.Row - rowIncrement, bishop.Col + colIncrement];
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

            var pieceCell = board.Cells[knight.Row, knight.Col];
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

            while (IsCellValid(rook.Row - increment, rook.Col, board))
            {
                var currCell = board.Cells[rook.Row - increment, rook.Col];
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
                            while (IsCellValid(rook.Row - ++increment, rook.Col, board))
                            {
                                currCell = board.Cells[rook.Row - increment, rook.Col];
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
            while (IsCellValid(rook.Row + increment, rook.Col, board))
            {
                var currCell = board.Cells[rook.Row + increment, rook.Col];
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
                            while (IsCellValid(rook.Row + ++increment, rook.Col, board))
                            {
                                currCell = board.Cells[rook.Row + increment, rook.Col];
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
            while (IsCellValid(rook.Row, rook.Col - increment, board))
            {
                var currCell = board.Cells[rook.Row, rook.Col - increment];
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
                            while (IsCellValid(rook.Row, rook.Col - ++increment, board))
                            {
                                currCell = board.Cells[rook.Row, rook.Col - increment];
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
            while (IsCellValid(rook.Row, rook.Col + increment, board))
            {
                var currCell = board.Cells[rook.Row, rook.Col + increment];
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
                            while (IsCellValid(rook.Row, rook.Col + ++increment, board))
                            {
                                currCell = board.Cells[rook.Row, rook.Col + increment];
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
            var queenLegalMoves = new Dictionary<string, List<Cell>>
            {
                { LegalMovesAndProtectedCells.LegalMoves, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Union(bishopLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]).ToList() },
                { LegalMovesAndProtectedCells.ProtectedCells, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells].Union(bishopLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells]).ToList() }
            };

            return queenLegalMoves;
        }

        private static Dictionary<string, List<Cell>> GetKnookLegalMovesAndProtectedCells(Piece knook, Board board)
        {
            var rookLegalMovesAndProtectedCells = GetRookLegalMovesAndProtectedCells(knook, board);
            var knightLegalMoves = GetKnightLegalMovesAndProtectedCells(knook, board);
            var knookLegalMoves = new Dictionary<string, List<Cell>>
            {
                { LegalMovesAndProtectedCells.LegalMoves, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Union(knightLegalMoves[LegalMovesAndProtectedCells.LegalMoves]).ToList() },
                { LegalMovesAndProtectedCells.ProtectedCells, rookLegalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells].Union(knightLegalMoves[LegalMovesAndProtectedCells.ProtectedCells]).ToList() }
            };

            return knookLegalMoves;
        }

        private static Dictionary<string, List<Cell>> GetKingLegalMovesAndProtectedCells(Piece king, Board board)
        {
            var legalMovesAndProtectedCells = new Dictionary<string, List<Cell>>();
            var legalMoves = new List<Cell>();

            var protectedCells = new List<Cell>();
            var pieceCell = board.Cells[king.Row, king.Col];
            var currCell = board.Cells[king.Row, king.Col];
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

            if (!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(lm => lm.Row == king.Row && lm.Col == king.Col))
                && !board.Moves.Any(m => m.CellTwoAfter.Piece!.PieceType == PieceType.King && m.CellTwoAfter.Piece.Color == king.Color)
                && king.Col == 4)
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
            var castlingRights = king.Color == PieceColor.White
                ? new bool[2] { board.StartingPosition.CastlingRights.Item1, board.StartingPosition.CastlingRights.Item2 }
                : new bool[2] { board.StartingPosition.CastlingRights.Item3, board.StartingPosition.CastlingRights.Item4 };
            if (castlingRights[0] && board.Cells[king.Row, king.Col + 3].Piece != null
                && board.Cells[king.Row, king.Col + 3].Piece!.PieceType == PieceType.Rook)
            {
                var rook = board.Cells[king.Row, king.Col + 3].Piece;
                bool wayIsClear = true;
                for (int i = king.Col + 1; i < board.Cells.GetLength(1) - 1; i++)
                {
                    if (board.Cells[king.Row, i].Piece != null)
                    {
                        wayIsClear = false;
                        break;
                    }
                }

                if (!board.Moves.Any(m => m.CellOneBefore.Row == rook!.Row && m.CellOneBefore.Col == rook.Col) && wayIsClear)
                {
                    if (!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col + 2))
                    && !board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col + 1))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col + 2))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col + 1)))
                    {
                        legalCastlingMoves.Add(board.Cells[king.Row, king.Col + 2]);
                    }
                }
            }

            if (castlingRights[1] && board.Cells[king.Row, king.Col - 4].Piece != null
                && board.Cells[king.Row, king.Col - 4].Piece!.PieceType == PieceType.Rook)
            {
                var rook = board.Cells[king.Row, king.Col - 4].Piece;

                bool wayIsClear = true;
                for (int i = king.Col - 1; i > 1; i--)
                {
                    if (board.Cells[king.Row, i].Piece != null)
                    {
                        wayIsClear = false;
                        break;
                    }
                }
                if (!board.Moves.Any(m => m.CellOneBefore.Row == rook!.Row && m.CellOneBefore.Col == rook.Col) && wayIsClear)
                {
                    if (!board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col - 2))
                    && !board.Pieces[oppositeColor].Any(p => p.LegalMoves.Any(m => m.Row == king.Row && m.Col == king.Col - 1))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col - 2))
                    && !board.Pieces[oppositeColor].Any(p => p.ProtectedCells.Any(m => m.Row == king.Row && m.Col == king.Col - 1)))
                    {
                        legalCastlingMoves.Add(board.Cells[king.Row, king.Col - 2]);
                    }
                }
            }
            return legalCastlingMoves;
        }

        private static bool IsCellValid(int row, int col, Board board)
        {
            return row >= 0 && row < board.Cells.GetLength(0) && col >= 0 && col < board.Cells.GetLength(1);
        }

        private static Cell GetCellFromAnnotation(Board board, string annotation)
        {
            return board.Cells[8 - (int)(annotation[1] - 48), (int)(annotation[0] - 97)];
        }
    }
}
