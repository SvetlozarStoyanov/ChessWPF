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
            if (move.CellOneBefore.Piece.PieceType != PieceType.Pawn && pieces[move.CellOneBefore.Piece.Color].Count(p => p.PieceType == move.CellOneBefore.Piece.PieceType) > 1)
            {
                var otherPiecesWhoCanDoThisMove = pieces[move.CellOneBefore.Piece.Color].Where(p => p.PieceType == move.CellOneBefore.Piece.PieceType
                && p.LegalMoves.Any(lm => lm.HasEqualRowAndCol(move.CellTwoBefore)) && !p.Equals(move.CellOneBefore.Piece));
                if (otherPiecesWhoCanDoThisMove.Count() > 1)
                {
                    sb.Append(AnnotatePiece(move.CellOneBefore, otherPiecesWhoCanDoThisMove));
                }
                else
                {

                    sb.Append(AnnotatePiece(move.CellOneBefore));
                }
            }
            else
            {
                sb.Append(AnnotatePiece(move.CellOneBefore));
            }
            sb.Append(AnnotateCapture(move));
            sb.Append(AnnotateCell(move.CellTwoAfter));
            if (move.IsPromotionMove)
            {
                sb.Append(AnnotatePromotion(move.CellTwoAfter));
            }
            return sb.ToString();
        }

        private static string AnnotatePiece(Cell cell)
        {
            var pieceAnnotation = AnnotatePieceType(cell.Piece.PieceType);
            return pieceAnnotation;
        }

        private static string AnnotatePiece(Cell movingPieceCell, IEnumerable<Piece> otherPiecesWhoHaveThisLegalMove)
        {
            var pieceAnnotation = new StringBuilder();
            pieceAnnotation.Append(AnnotatePieceType(movingPieceCell.Piece.PieceType));
            if (otherPiecesWhoHaveThisLegalMove.Any(p => (p.Cell.Row == movingPieceCell.Row)
                && otherPiecesWhoHaveThisLegalMove.All(p => p.Cell.Col != movingPieceCell.Col))
                || (otherPiecesWhoHaveThisLegalMove.All(p => p.Cell.Row != movingPieceCell.Row)
                && otherPiecesWhoHaveThisLegalMove.All(p => p.Cell.Col != movingPieceCell.Col)))
            {
                pieceAnnotation.Append($"{Convert.ToChar(97 + movingPieceCell.Col)}");
            }
            else if (otherPiecesWhoHaveThisLegalMove.Any(p => (p.Cell.Col == movingPieceCell.Col)
                && otherPiecesWhoHaveThisLegalMove.All(p => p.Cell.Row != movingPieceCell.Row))
                || (otherPiecesWhoHaveThisLegalMove.All(p => p.Cell.Row != movingPieceCell.Row)
                && otherPiecesWhoHaveThisLegalMove.All(p => p.Cell.Col != movingPieceCell.Col)))
            {
                pieceAnnotation.Append($"{8 - movingPieceCell.Row}");
            }
            else if (otherPiecesWhoHaveThisLegalMove.Any(p => p.Cell.Col == movingPieceCell.Col)
                && otherPiecesWhoHaveThisLegalMove.Any(p => p.Cell.Row == movingPieceCell.Row))
            {
                pieceAnnotation.Append($"{Convert.ToChar(97 + movingPieceCell.Col)}{8 - movingPieceCell.Row}");
            }
            return pieceAnnotation.ToString();
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

        private static string AnnotatePromotion(Cell cell)
        {
            var promotedPieceAnnotation = AnnotatePieceType(cell.Piece.PieceType);
            return $"={promotedPieceAnnotation}";
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

        private static string AnnotatePieceType(PieceType pieceType)
        {
            var pieceAnnotation = string.Empty;
            switch (pieceType)
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
    }
}
