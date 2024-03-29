﻿using ChessWPF.Models.Boards;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Moves;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Linq;
using System.Text;

namespace ChessWPF.Game
{
    public static class FenAnnotationWriter
    {
        public static string WriteFenAnnotationFromBoard(Board board)
        {
            var sb = new StringBuilder();
            AnnotateRows(sb, board.Cells);
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" ");
            AnnotateTurnColor(sb, board.TurnColor);
            sb.Append(" ");
            AnnotateCastlingRights(sb, board);
            sb.Append(" ");
            if (board.Moves.Any())
            {
                AnnotatePossibleEnPassant(sb, board.Moves.Peek());
            }
            else
            {
                sb.Append("-");
            }
            sb.Append(" ");
            AnnotateHalfMoveCount(sb, board);
            sb.Append(" ");
            AnnotateFullMoveCount(sb, board);
            return sb.ToString();
        }

        public static string WriteFenAnnotationFromBoardConstructor(char[,] cells,
            PieceColor turnColor,
            bool[] castlingRights,
            CellCoordinates? enPassantCoordinates,
            int halfMoveCount,
            int moveNumber)
        {
            var sb = new StringBuilder();
            AnnotateRows(cells, sb);
            sb.Remove(sb.Length - 1, 1);
            sb.Append(" ");
            AnnotateTurnColor(sb, turnColor);
            sb.Append(" ");
            AnnotateCastlingRights(sb, castlingRights);
            sb.Append(" ");
            if (enPassantCoordinates != null)
            {
                AnnotateEnPassant(sb, enPassantCoordinates);
            }
            else
            {
                sb.Append("-");
            }
            sb.Append(" ");

            sb.Append(halfMoveCount);
            sb.Append(" ");
            sb.Append(moveNumber);
            return sb.ToString();
        }



        private static void AnnotateRows(StringBuilder sb, Cell[,] cells)
        {
            for (int row = 0; row < cells.GetLength(0); row++)
            {
                var emptyCellCount = 0;
                for (int col = 0; col < cells.GetLength(0); col++)
                {
                    if (cells[row, col].Piece == null)
                    {
                        emptyCellCount++;
                    }
                    else
                    {
                        var letter = ' ';
                        switch (cells[row, col].Piece!.PieceType)
                        {
                            case PieceType.Pawn:
                                letter = 'p';
                                break;
                            case PieceType.Knight:
                                letter = 'n';
                                break;
                            case PieceType.Bishop:
                                letter = 'b';
                                break;
                            case PieceType.Rook:
                                letter = 'r';
                                break;
                            case PieceType.Queen:
                                letter = 'q';
                                break;
                            case PieceType.Knook:
                                letter = 'o';
                                break;
                            case PieceType.King:
                                letter = 'k';
                                break;
                        }
                        if (cells[row, col].Piece!.Color == PieceColor.White)
                        {
                            letter = (char)(letter - 32);
                        }
                        if (emptyCellCount > 0)
                        {
                            sb.Append($"{emptyCellCount}{letter}");
                            emptyCellCount = 0;
                        }
                        else
                        {
                            sb.Append($"{letter}");
                        }
                    }
                }
                if (emptyCellCount > 0)
                {
                    sb.Append($"{emptyCellCount}/");
                }
                else
                {
                    sb.Append($"/");
                }
            }
        }

        private static void AnnotateCastlingRights(StringBuilder sb, Board board)
        {
            var castlingRights = new bool[4];
            var hasWhiteKingMoved = board.Moves.Any(m => m.CellOneBefore.Piece!.PieceType == PieceType.King
                && m.CellOneBefore.Piece.Color == PieceColor.White);

            var hasBlackKingMoved = board.Moves.Any(m => m.CellOneBefore.Piece!.PieceType == PieceType.King
                && m.CellOneBefore.Piece.Color == PieceColor.Black);

            if (!hasWhiteKingMoved)
            {
                if (board.StartingPosition.CastlingRights[0] &&
                    !board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 7 && m.CellOneBefore.Col == 7) ||
                (m.CellTwoBefore.Row == 7 && m.CellTwoBefore.Col == 7))) &&
                (board.Cells[7, 7].Piece != null && board.Cells[7, 7].Piece!.PieceType == PieceType.Rook &&
                board.Cells[7, 7].Piece!.Color == PieceColor.White))
                {
                    castlingRights[0] = true;
                }
                if (board.StartingPosition.CastlingRights[1] &&
                    !board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 7 && m.CellOneBefore.Col == 0) ||
                (m.CellTwoBefore.Row == 7 && m.CellTwoBefore.Col == 0))) &&
                (board.Cells[7, 0].Piece != null && board.Cells[7, 0].Piece!.PieceType == PieceType.Rook &&
                board.Cells[7, 0].Piece!.Color == PieceColor.White))
                {
                    castlingRights[1] = true;
                }
            }

            if (!hasBlackKingMoved)
            {
                if (board.StartingPosition.CastlingRights[2] &&
                    !board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 0 && m.CellOneBefore.Col == 7) ||
                (m.CellTwoBefore.Row == 0 && m.CellTwoBefore.Col == 7))) &&
                (board.Cells[0, 7].Piece != null && board.Cells[0, 7].Piece!.PieceType == PieceType.Rook &&
                board.Cells[0, 7].Piece!.Color == PieceColor.Black))
                {
                    castlingRights[2] = true;
                }
                if (board.StartingPosition.CastlingRights[3] &&
                    !board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 0 && m.CellOneBefore.Col == 0) ||
                (m.CellTwoBefore.Row == 0 && m.CellTwoBefore.Col == 0))) &&
                (board.Cells[0, 0].Piece != null && board.Cells[0, 0].Piece!.PieceType == PieceType.Rook &&
                board.Cells[0, 0].Piece!.Color == PieceColor.Black))
                {
                    castlingRights[3] = true;
                }
            }
            AnnotateCastlingRights(sb, castlingRights);
        }

        private static void AnnotatePossibleEnPassant(StringBuilder sb, Move move)
        {
            if (move.CellOneBefore.Piece!.PieceType == PieceType.Pawn && Math.Abs(move.CellTwoBefore.Row - move.CellOneBefore.Row) == 2)
            {
                var columnAsLetter = (char)(97 + move.CellOneBefore.Col);
                var row = move.CellOneBefore.Row == 1 ? 6 : 3;
                sb.Append($"{columnAsLetter}{row}");
            }
            else
            {
                sb.Append("-");
            }
        }

        private static void AnnotateHalfMoveCount(StringBuilder sb, Board board)
        {
            sb.Append($"{(board.HalfMoveCount > 0 ? board.HalfMoveCount : 0)}");
        }

        private static void AnnotateFullMoveCount(StringBuilder sb, Board board)
        {
            sb.Append($"{(board.Moves.Count < 2 ? 1 : board.Moves.Count(m => m.CellOneBefore.Piece!.Color == PieceColor.Black) + 1)}");
        }

        private static void AnnotateEnPassant(StringBuilder sb, CellCoordinates? enPassantCoordinates)
        {
            var row = 8 - enPassantCoordinates.Value.Row;
            var col = (char)(enPassantCoordinates!.Value.Col + 97);
            sb.Append($"{col}{row}");
        }

        private static void AnnotateCastlingRights(StringBuilder sb, bool[] castlingRights)
        {
            if (castlingRights[0] == false
                && castlingRights[1] == false
                && castlingRights[2] == false
                && castlingRights[3] == false)
            {
                sb.Append("-");
                return;
            }
            if (castlingRights[0])
            {
                sb.Append("K");
            }
            if (castlingRights[1])
            {
                sb.Append("Q");
            }
            if (castlingRights[2])
            {
                sb.Append("k");
            }
            if (castlingRights[3])
            {
                sb.Append("q");
            }
        }

        private static void AnnotateTurnColor(StringBuilder sb, PieceColor turnColor)
        {
            sb.Append($"{(turnColor == PieceColor.White ? "w" : "b")}");
        }

        private static void AnnotateRows(char[,] cells, StringBuilder sb)
        {
            for (int row = 0; row < cells.GetLength(0); row++)
            {
                var emptyCellCount = 0;
                for (int col = 0; col < cells.GetLength(0); col++)
                {
                    if (cells[row, col] == '.')
                    {
                        emptyCellCount++;
                    }
                    else
                    {
                        if (emptyCellCount > 0)
                        {
                            sb.Append($"{emptyCellCount}{cells[row, col]}");
                            emptyCellCount = 0;
                        }
                        else
                        {
                            sb.Append($"{cells[row, col]}");
                        }
                    }
                }
                if (emptyCellCount > 0)
                {
                    sb.Append($"{emptyCellCount}/");
                }
                else
                {
                    sb.Append($"/");
                }
            }
        }
    }
}
