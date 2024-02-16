using ChessWPF.Constants;
using ChessWPF.HelperClasses.Exceptions;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using ChessWPF.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Game
{
    public static class PositionValidator
    {
        private static Position position;

        public static bool ValidatePositionFromFenAnnotation(string fenAnnotation)
        {
            position = FenAnnotationReader.GetPosition(fenAnnotation);
            ValidatePosition(position);
            return true;
        }

        private static bool ValidatePosition(Position positionInput)
        {
            position = positionInput;

            if (!ValidatePieceComposition())
            {
                throw new InvalidPositionException("Invalid Piece Composition!");
            }

            if (!ValidateCastlingRights())
            {
                throw new InvalidPositionException("Invalid castling eights!");
            }
            if (!(position.EnPassantCoordinates != null ? ValidateEnPassant() : true))
            {
                throw new InvalidPositionException("Invalid en passant!");
            }
            if (!ValidateOppositeTurnColorKingCannotBeTaken())
            {
                throw new InvalidPositionException($"{(position.TurnColor == PieceColor.White ? "Black" : "White")} King can be taken!");
            }
            if (!ValidateNoPawnsAreOnFirstRank())
            {
                throw new InvalidPositionException("No pawns can be on the back rank!");
            }
            return true;
        }

        private static bool ValidatePieceComposition()
        {
            if (!ValidateKingCount())
            {
                return false;
            }
            if (!ValidatePiecesAreSufficientForPlay())
            {
                return false;
            }
            return true;
        }

        private static bool ValidateKingCount()
        {
            foreach (var color in position.Pieces.Keys)
            {
                var kingCount = position.Pieces[color].Count(p => p.PieceType == PieceType.King);
                if (kingCount == 0 || kingCount > 1)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidatePiecesAreSufficientForPlay()
        {
            var whitePieces = position.Pieces[PieceColor.White];
            var blackPieces = position.Pieces[PieceColor.Black];
            if (whitePieces.Count == 1 && blackPieces.Count == 1)
            {
                return false;
            }
            if (whitePieces.Count == 2 && whitePieces
                .Any(p => p.PieceType == PieceType.Knight
                    || p.PieceType == PieceType.Bishop)
                && blackPieces.Count == 1)
            {
                return false;
            }
            if (blackPieces.Count == 2 && blackPieces
                .Any(p => p.PieceType == PieceType.Knight
                    || p.PieceType == PieceType.Bishop)
                && whitePieces.Count == 1)
            {
                return false;
            }

            return true;
        }

        private static bool ValidateCastlingRights()
        {
            var castlingRights = position.CastlingRights;
            var simplifiedCells = position.SimplifiedCells;
            if (castlingRights.Item1 == true)
            {
                if (simplifiedCells[7, 4] != 'K' || simplifiedCells[7, 7] != 'R')
                {
                    return false;
                }
            }
            if (castlingRights.Item2 == true)
            {
                if (simplifiedCells[7, 4] != 'K' || simplifiedCells[7, 0] != 'R')
                {
                    return false;
                }
            }
            if (castlingRights.Item3 == true)
            {
                if (simplifiedCells[0, 4] != 'k' || simplifiedCells[0, 7] != 'r')
                {
                    return false;
                }
            }
            if (castlingRights.Item4 == true)
            {
                if (simplifiedCells[0, 4] != 'k' || simplifiedCells[0, 0] != 'r')
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ValidateEnPassant()
        {
            var enPassantRow = position.EnPassantCoordinates!.Value.Row;
            var enPassantCol = position.EnPassantCoordinates.Value.Col;

            if (enPassantRow == 2 && position.SimplifiedCells[enPassantRow, enPassantCol] == '.'
                && position.SimplifiedCells[enPassantRow + 1, enPassantCol] == 'p'
                && position.SimplifiedCells[enPassantRow - 1, enPassantCol] == '.')
            {
                return true;
            }
            else if (enPassantRow == 5 && position.SimplifiedCells[enPassantRow, enPassantCol] == '.'
                && position.SimplifiedCells[enPassantRow - 1, enPassantCol] == 'P'
                && position.SimplifiedCells[enPassantRow + 1, enPassantCol] == '.')
            {
                return true;
            }
            return false;
        }

        private static bool ValidateOppositeTurnColorKingCannotBeTaken()
        {
            var oppositeTurnColor = position.TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var oppositeTurnColorKing = position.Pieces[oppositeTurnColor].FirstOrDefault(p => p.PieceType == PieceType.King);
            if (CheckRowsAndColumnsForAttackers(oppositeTurnColorKing!.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }
            if (CheckDiagonalsForAttackers(oppositeTurnColorKing.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }
            if (CheckForKnightAttackers(oppositeTurnColorKing.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }
            if (CheckForPawnAttackers(oppositeTurnColorKing.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }
            if (CheckForKingAttackers(oppositeTurnColorKing.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }

            return true;
        }

        private static bool CheckRowsAndColumnsForAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'R', 'O', 'Q' } : new char[] { 'r', 'o', 'q' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            var increment = 1;
            while (CellIsValid(row + increment, col))
            {
                if (char.IsLetter(cells[row + increment, col]))
                {
                    if (isAttacker(cells[row + increment, col]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                increment++;
            }
            increment = 1;
            while (CellIsValid(row - increment, col))
            {
                if (char.IsLetter(cells[row - increment, col]))
                {
                    if (isAttacker(cells[row - increment, col]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                increment++;
            }
            increment = 1;
            while (CellIsValid(row, col + increment))
            {
                if (char.IsLetter(cells[row, col + increment]))
                {
                    if (isAttacker(cells[row, col + increment]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                increment++;
            }
            increment = 1;
            while (CellIsValid(row, col - increment))
            {
                if (char.IsLetter(cells[row, col - increment]))
                {
                    if (isAttacker(cells[row, col - increment]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                increment++;
            }
            return false;
        }

        private static bool CheckDiagonalsForAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'B', 'Q' } : new char[] { 'b', 'q' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            var rowIncrement = 1;
            var colIncrement = 1;
            while (CellIsValid(row + rowIncrement, col + colIncrement))
            {
                if (char.IsLetter(cells[row + rowIncrement, col + colIncrement]))
                {
                    if (isAttacker(cells[row + rowIncrement, col + colIncrement]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (CellIsValid(row + rowIncrement, col - colIncrement))
            {
                if (char.IsLetter(cells[row + rowIncrement, col - colIncrement]))
                {
                    if (isAttacker(cells[row + rowIncrement, col - colIncrement]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (CellIsValid(row - rowIncrement, col + colIncrement))
            {
                if (char.IsLetter(cells[row - rowIncrement, col + colIncrement]))
                {
                    if (isAttacker(cells[row - rowIncrement, col + colIncrement]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                rowIncrement++;
                colIncrement++;
            }
            rowIncrement = 1;
            colIncrement = 1;
            while (CellIsValid(row - rowIncrement, col - colIncrement))
            {
                if (char.IsLetter(cells[row - rowIncrement, col - colIncrement]))
                {
                    if (isAttacker(cells[row - rowIncrement, col - colIncrement]))
                    {
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                rowIncrement++;
                colIncrement++;
            }
            return false;
        }

        private static bool CheckForKnightAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'N', 'O' } : new char[] { 'n', 'o' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            if (CellIsValid(row + 2, col + 1) && isAttacker(cells[row + 2, col + 1]))
            {
                return true;
            }
            if (CellIsValid(row + 2, col - 1) && isAttacker(cells[row + 2, col - 1]))
            {
                return true;
            }
            if (CellIsValid(row - 2, col + 1) && isAttacker(cells[row - 2, col + 1]))
            {
                return true;
            }
            if (CellIsValid(row - 2, col - 1) && isAttacker(cells[row - 2, col - 1]))
            {
                return true;
            }
            if (CellIsValid(row + 1, col + 2) && isAttacker(cells[row + 1, col + 2]))
            {
                return true;
            }
            if (CellIsValid(row + 1, col - 2) && isAttacker(cells[row + 1, col - 2]))
            {
                return true;
            }
            if (CellIsValid(row - 1, col + 2) && isAttacker(cells[row - 1, col + 2]))
            {
                return true;
            }
            if (CellIsValid(row - 1, col - 2) && isAttacker(cells[row - 1, col - 2]))
            {
                return true;
            }
            return false;
        }

        private static bool CheckForPawnAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'P' } : new char[] { 'p' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            if (attackerColor == PieceColor.White)
            {
                if (CellIsValid(row + 1, col - 1) && isAttacker(cells[row + 1, col - 1]))
                {
                    return true;
                }
                if (CellIsValid(row + 1, col + 1) && isAttacker(cells[row + 1, col + 1]))
                {
                    return true;
                }
            }
            else
            {
                if (CellIsValid(row - 1, col - 1) && isAttacker(cells[row - 1, col - 1]))
                {
                    return true;
                }
                if (CellIsValid(row - 1, col + 1) && isAttacker(cells[row - 1, col + 1]))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CheckForKingAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'K' } : new char[] { 'k' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);

            if (CellIsValid(row - 1, col - 1) && isAttacker(cells[row - 1, col - 1]))
            {
                return true;
            }
            if (CellIsValid(row - 1, col) && isAttacker(cells[row - 1, col]))
            {
                return true;
            }
            if (CellIsValid(row - 1, col + 1) && isAttacker(cells[row - 1, col + 1]))
            {
                return true;
            }
            if (CellIsValid(row, col - 1) && isAttacker(cells[row, col - 1]))
            {
                return true;
            }
            if (CellIsValid(row, col + 1) && isAttacker(cells[row, col + 1]))
            {
                return true;
            }
            if (CellIsValid(row + 1, col + 1) && isAttacker(cells[row + 1, col + 1]))
            {
                return true;
            }
            if (CellIsValid(row + 1, col) && isAttacker(cells[row + 1, col]))
            {
                return true;
            }
            if (CellIsValid(row + 1, col + 1) && isAttacker(cells[row + 1, col + 1]))
            {
                return true;
            }

            return false;
        }

        private static bool ValidateNoPawnsAreOnFirstRank()
        {
            if (!(position.Pieces.Values.Any(p => p.Any(p => p.PieceType == PieceType.Pawn))))
            {
                return true;
            }
            for (int row = 0; row < position.SimplifiedCells.GetLength(0); row += 7)
            {
                for (int col = 0; col < position.SimplifiedCells.GetLength(1); col++)
                {
                    if (position.SimplifiedCells[row, col] == 'p' || position.SimplifiedCells[row, col] == 'P')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool CellIsValid(int row, int col)
        {
            return row >= 0 && row < position.SimplifiedCells.GetLength(0) &&
                col >= 0 && col < position.SimplifiedCells.GetLength(1);
        }

        //public static void CalculatePossibleMoves()
        //{
        //    var pieces = position.Pieces;
        //    var turnColor = position.TurnColor;
        //    var oppositeColor = turnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
        //    var oppositeKing = (King)pieces[oppositeColor].First(p => p.PieceType == PieceType.King);
            
        //    var king = (King)pieces[turnColor].First(p => p.PieceType == PieceType.King);
        //    king.Defenders = KingDefenderFinder.FindDefenders(king, turnColor, this);
        //    king.Attackers.Clear();
        //    foreach (var piece in pieces[oppositeColor])
        //    {
        //        var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece, this);
        //        piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
        //        piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
        //        Cells[piece.Row, piece.Col].Piece = piece;
        //        var checkedKingCell = piece.LegalMoves.FirstOrDefault(c => c.Piece != null && c.Piece.PieceType == PieceType.King);
        //        if (checkedKingCell != null)
        //        {
        //            king.Attackers.Add(piece);
        //        }
        //    }
        //    if (king.Attackers.Any())
        //    {
                
        //        king.IsInCheck = true;
                
        //    }
        //    var validMovesToStopCheck = new List<Cell>();
        //    if (king.Attackers.Count == 1)
        //    {
        //        validMovesToStopCheck = LegalMovesToStopCheckFinder.GetLegalMovesToStopCheck(king, king.Attackers.First(), this);
        //    }
        //    if (king.Attackers.Count > 1)
        //    {
        //        var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(king, this);
        //        king.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
        //        foreach (var piece in pieces[TurnColor].Where(p => p.PieceType != PieceType.King))
        //        {
        //            piece.LegalMoves = new List<Cell>();
        //            piece.ProtectedCells = new List<Cell>();
        //        }
        //    }
        //    else
        //    {
        //        foreach (var piece in pieces[TurnColor])
        //        {
        //            var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece, this);
        //            if (validMovesToStopCheck.Count > 0 && piece.PieceType != PieceType.King && !king.Defenders.Any(d => d.Item1 == piece))
        //            {
        //                legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]
        //                    .Where(lm => validMovesToStopCheck.Contains(lm)).ToList();
        //            }
        //            else if (king.Defenders.Any(d => d.Item1 == piece))
        //            {
        //                if (king.IsInCheck)
        //                {
        //                    legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = new List<Cell>();
        //                }
        //                var currDefenderAndPotentialAttacker = king.Defenders.First(d => d.Item1 == piece);
        //                var movesToPreventPotentialCheck = LegalMovesToStopCheckFinder.GetLegalMovesToStopCheck(king, currDefenderAndPotentialAttacker.Item2, this);
        //                movesToPreventPotentialCheck.Remove(movesToPreventPotentialCheck.FirstOrDefault(c => c.Row == currDefenderAndPotentialAttacker.Item1.Row && c.Col == currDefenderAndPotentialAttacker.Item1.Col)!);
        //                movesToPreventPotentialCheck.Add(Cells[currDefenderAndPotentialAttacker.Item2.Row, currDefenderAndPotentialAttacker.Item2.Col]);
        //                if (king.Attackers.Count == 1 && king.Attackers.First().PieceType == PieceType.Knight)
        //                {
        //                    legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = new List<Cell>();
        //                }
        //                legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Where(lm => movesToPreventPotentialCheck.Contains(lm)).ToList();
        //            }
        //            piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
        //            piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
        //            Cells[piece.Row, piece.Col].Piece!.UpdateLegalMovesAndProtectedCells(piece.LegalMoves, piece.ProtectedCells);
        //        }
        //    }
        //}
    }
}
