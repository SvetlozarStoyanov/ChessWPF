using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Models.Data.Pieces;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace ChessWPF.Game
{
    public static class MoveNotationWriter
    {
        public static string AnnotateMove(Move move, Dictionary<PieceColor, List<Piece>> pieces)
        {
            var sb = new StringBuilder();
            if (move.CellOneBefore.Piece.PieceType == PieceType.King
                && move.CellThreeBefore != null)
            {
                return AnnotateCastling(move);
            }
            if (pieces[move.CellOneBefore.Piece.Color].Any(p => p.LegalMoves.Any(c => c.HasEqualRowAndCol(move.CellTwoBefore))))
            {
                sb.Append(AnnotatePiece(move.CellOneBefore, move.CellTwoBefore));
            }
            else
            {
                sb.Append(AnnotatePiece(move.CellOneBefore));
            }
            sb.Append(AnnotateCapture(move));
            sb.Append(AnnotateCell(move.CellTwoAfter));
            return sb.ToString();
        }

        private static string AnnotatePiece(Cell cell)
        {
            var pieceAnnotation = string.Empty;
            switch (cell.Piece.PieceType)
            {
                case PieceType.Pawn:
                    pieceAnnotation = "";
                    break;
                case PieceType.Knight:
                    pieceAnnotation = "N";
                    break;
                case PieceType.Bishop:
                    pieceAnnotation = "B";
                    break;
                case PieceType.Rook:
                    pieceAnnotation = "R";
                    break;
                case PieceType.Queen:
                    pieceAnnotation = "Q";
                    break;
                case PieceType.Knook:
                    pieceAnnotation = "O";
                    break;
                case PieceType.King:
                    pieceAnnotation = "K";
                    break;
            }
            return pieceAnnotation;
        }

        private static string AnnotatePiece(Cell movingPieceCell, Cell destinationCell)
        {
            var pieceAnnotation = string.Empty;
            switch (movingPieceCell.Piece.PieceType)
            {
                case PieceType.Pawn:
                    pieceAnnotation = "";
                    break;
                case PieceType.Knight:
                    pieceAnnotation = "n";
                    break;
                case PieceType.Bishop:
                    pieceAnnotation = "b";
                    break;
                case PieceType.Rook:
                    pieceAnnotation = "r";
                    break;
                case PieceType.Queen:
                    pieceAnnotation = "q";
                    break;
                case PieceType.Knook:
                    pieceAnnotation = "o";
                    break;
                case PieceType.King:
                    pieceAnnotation = "k";
                    break;
            }
            if (movingPieceCell.Row != destinationCell.Row)
            {
                pieceAnnotation += $"{8 - movingPieceCell.Row}";
            }
            else if (movingPieceCell.Col != destinationCell.Col)
            {
                pieceAnnotation += $"{Convert.ToChar(97 + movingPieceCell.Col)}";
            }
            return pieceAnnotation;
        }

        private static string AnnotateCapture(Move move)
        {
            if (move.CellTwoBefore.Piece != null || (move.CellThreeBefore != null && move.CellFourBefore == null))
            {
                return $"{(move.CellOneBefore.Piece.PieceType == PieceType.Pawn ? AnnotateColumn(move.CellOneBefore.Col)
                    : string.Empty)}x";
            }
            return string.Empty;
        }

        private static string AnnotateCell(Cell cell)
        {
            return $"{Convert.ToChar(97 + cell.Col)}{8 - cell.Row}";
        }

        private static string AnnotateCastling(Move move)
        {
            if (move.CellOneBefore.ColumnDifference(move.CellTwoBefore) == 2)
            {
                return "O-O";
            }
            if (move.CellOneBefore.ColumnDifference(move.CellTwoBefore) == 3)
            {
                return "O-O-O";
            }
            return string.Empty;
        }

        private static string AnnotateColumn(int col)
        {
            return $"{Convert.ToChar(97 + col)}";
        }
    }
}
