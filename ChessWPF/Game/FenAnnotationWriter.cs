using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Linq;
using System.Text;

namespace ChessWPF.Game
{
    public static class FenAnnotationWriter
    {
        public static string WriteFenAnnotation(Board board)
        {
            var sb = new StringBuilder();
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                var currRow = new Cell[8];
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    currRow[col] = board.Cells[row, col];
                }
                sb.Append(AnnotateRow(currRow));
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append($" {(board.TurnColor == PieceColor.White ? 'w' : 'b')} ");
            sb.Append(AnnotateCastlingRights(board));
            if (board.Moves.Any())
            {
                sb.Append(AnnotatePossibleEnPassant(board.Moves.Peek()));
            }
            else
            {
                sb.Append("- ");
            }
            sb.Append(AnnotateHalfMoveCount(board));
            sb.Append(AnnotateFullMoveCount(board));
            return sb.ToString();
        }
        private static string AnnotateRow(Cell[] row)
        {
            var sb = new StringBuilder();
            var emptyCells = 0;
            var letter = ' ';
            foreach (var cell in row)
            {
                if (cell.Piece != null)
                {
                    if (emptyCells > 0)
                    {
                        sb.Append(emptyCells);
                        emptyCells = 0;
                    }
                    switch (cell.Piece.PieceType)
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
                        case PieceType.King:
                            letter = 'k';
                            break;
                        case PieceType.Knook:
                            letter = 'o';
                            break;
                    }
                    if (cell.Piece.Color == PieceColor.White)
                    {
                        letter = (char)(letter - 32);
                    }
                    sb.Append(letter);
                }
                else
                {
                    emptyCells++;
                }
            }
            if (emptyCells > 0)
            {
                sb.Append(emptyCells);
                emptyCells = 0;
            }
            sb.Append('/');
            return sb.ToString();
        }

        private static string AnnotateCastlingRights(Board board)
        {
            var sb = new StringBuilder();
            var whiteCastlingRights = new StringBuilder();
            var blackCastlingRights = new StringBuilder();

            var hasWhiteKingMoved = board.Moves.Any(m => m.CellOneBefore.Piece!.PieceType == PieceType.King
                && m.CellOneBefore.Piece.Color == PieceColor.White);

            var hasBlackKingMoved = board.Moves.Any(m => m.CellOneBefore.Piece!.PieceType == PieceType.King
                && m.CellOneBefore.Piece.Color == PieceColor.Black);

            if (!hasWhiteKingMoved)
            {
                if (!board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 7 && m.CellOneBefore.Col == 7) ||
                (m.CellTwoBefore.Row == 7 && m.CellTwoBefore.Col == 7))) &&
                (board.Cells[7, 7].Piece != null && board.Cells[7, 7].Piece!.PieceType == PieceType.Rook &&
                board.Cells[7, 7].Piece!.Color == PieceColor.White))
                {
                    whiteCastlingRights.Append('K');
                }
                if (!board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 7 && m.CellOneBefore.Col == 0) ||
                (m.CellTwoBefore.Row == 7 && m.CellTwoBefore.Col == 0))) &&
                (board.Cells[7, 0].Piece != null && board.Cells[7, 0].Piece!.PieceType == PieceType.Rook &&
                board.Cells[7, 0].Piece!.Color == PieceColor.White))
                {
                    whiteCastlingRights.Append('Q');
                }
            }

            if (!hasBlackKingMoved)
            {
                if (!board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 0 && m.CellOneBefore.Col == 7) ||
                (m.CellTwoBefore.Row == 0 && m.CellTwoBefore.Col == 7))) &&
                (board.Cells[0, 7].Piece != null && board.Cells[0, 7].Piece!.PieceType == PieceType.Rook &&
                board.Cells[0, 7].Piece!.Color == PieceColor.Black))
                {
                    blackCastlingRights.Append('k');
                }
                if (!board.Moves.Any(m =>
                ((m.CellOneBefore.Row == 0 && m.CellOneBefore.Col == 0) ||
                (m.CellTwoBefore.Row == 0 && m.CellTwoBefore.Col == 0))) &&
                (board.Cells[0, 0].Piece != null && board.Cells[0, 0].Piece!.PieceType == PieceType.Rook &&
                board.Cells[0, 0].Piece!.Color == PieceColor.Black))
                {
                    blackCastlingRights.Append('q');
                }
            }

            if (whiteCastlingRights.ToString() == "" && blackCastlingRights.ToString() == "")
            {
                return "- ";
            }
            sb.Append($"{whiteCastlingRights.ToString()}{blackCastlingRights.ToString()} ");
            return sb.ToString();
        }

        private static string AnnotatePossibleEnPassant(Move move)
        {
            if (move.CellOneBefore.Piece!.PieceType == PieceType.Pawn && Math.Abs(move.CellTwoBefore.Row - move.CellOneBefore.Row) == 2)
            {
                var columnAsLetter = (char)(97 + move.CellOneBefore.Col);
                var row = move.CellOneBefore.Row == 1 ? 6 : 3;
                return $"{columnAsLetter}{row} ";
            }
            return "- ";
        }

        private static string AnnotateHalfMoveCount(Board board)
        {
            return $"{(board.HalfMoveCount > 0 ? board.HalfMoveCount : 0)} ";
        }

        private static string AnnotateFullMoveCount(Board board)
        {
            return $"{(board.Moves.Count < 2 ? 1 : board.Moves.Count(m => m.CellOneBefore.Piece!.Color == PieceColor.Black) + 1)}";
        }
    }
}
