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
                throw new InvalidPositionException("Invalid piece composition!");
            }
            if (!ValidateCastlingRights())
            {
                throw new InvalidPositionException("Invalid castling rights!");
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
            if (!ValidateColorToMoveHasLegalMoves())
            {
                throw new InvalidPositionException("Game is already over!");
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
            if (castlingRights[0] == true)
            {
                if (simplifiedCells[7, 4] != 'K' || simplifiedCells[7, 7] != 'R')
                {
                    return false;
                }
            }
            if (castlingRights[1] == true)
            {
                if (simplifiedCells[7, 4] != 'K' || simplifiedCells[7, 0] != 'R')
                {
                    return false;
                }
            }
            if (castlingRights[2] == true)
            {
                if (simplifiedCells[0, 4] != 'k' || simplifiedCells[0, 7] != 'r')
                {
                    return false;
                }
            }
            if (castlingRights[3] == true)
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
            return !AttackerFinder.HasAttackers(oppositeTurnColorKing!.Row,
                oppositeTurnColorKing.Col,
                position.TurnColor,
                position.SimplifiedCells);
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

        private static bool ValidateColorToMoveHasLegalMoves()
        {
            return ColorHasLegalMoves(position.TurnColor);
        }

        private static bool CellIsValid(int row, int col)
        {
            return row >= 0 && row < position.SimplifiedCells.GetLength(0) &&
                col >= 0 && col < position.SimplifiedCells.GetLength(1);
        }

        private static bool ColorHasLegalMoves(PieceColor turnColor)
        {
            var pieces = position.Pieces;
            var cells = position.SimplifiedCells;
            var oppositeColor = turnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var oppositeKing = (King)pieces[oppositeColor].First(p => p.PieceType == PieceType.King);

            var king = (King)pieces[turnColor].First(p => p.PieceType == PieceType.King);
            var defenders = KingDefenderFinder.FindDefendersSimplified(king, turnColor, position.Pieces, position.SimplifiedCells);
            var attackers = AttackerFinder.GetAttackersAndInterceptingMoves(king.Row, king.Col, oppositeColor, cells, pieces);
            var kingHasMoves = CheckIfKingHasMoves(king, oppositeColor, attackers, cells);
            if (attackers.Count > 0)
            {
                if (kingHasMoves)
                {
                    return true;
                }
                if (attackers.Count < 2)
                {
                    var canBlockCheck = CheckIfThereAreMovesToStopCheck(turnColor,
                        defenders,
                        attackers,
                        cells,
                        pieces,
                        position.EnPassantCoordinates);
                    if (canBlockCheck)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                foreach (var piece in pieces[turnColor].Where(p => p.PieceType != PieceType.King))
                {
                    if (defenders.Any(d => d.Item1.HasEqualCoordinates(piece.Row, piece.Col)))
                    {
                        var defenderAndPinningPiece = defenders.FirstOrDefault(d => d.Item1.HasEqualCoordinates(piece.Row, piece.Col));
                        var currAttackers = AttackerFinder.GetAttackersAndInterceptingMoves(piece.Row,
                            piece.Col,
                            oppositeColor,
                            cells,
                            pieces);
                        var pinningPiece = defenderAndPinningPiece.Item2;
                        var pinningAttacker = currAttackers.FirstOrDefault(a => a.Item1.HasEqualCoordinates(pinningPiece.Row, pinningPiece.Col));
                        var hasLegalMoves = LegalMoveFinder.HasLegalMoves(piece, cells, position.EnPassantCoordinates, pinningAttacker.Item2);
                        if (hasLegalMoves)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        var hasLegalMoves = LegalMoveFinder.HasLegalMoves(piece, cells, position.EnPassantCoordinates, null);
                        if (hasLegalMoves)
                        {
                            return true;
                        }
                    }
                }
                if (kingHasMoves)
                {
                    return true;
                }
                return false;
            }
        }

        private static bool CheckIfKingHasMoves(King king, PieceColor oppositeColor,
            List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>> attackers,
            char[,] cells)
        {
            Func<int, int, bool> canBeMovedTo = (row, col) => !AttackerFinder.HasAttackers(row, col, oppositeColor, cells)
                && attackers.All(att => att.Item3.HasValue ? !att.Item3.Value.HasEqualCoordinates(row, col) : true)
                && (cells[row, col] == '.'
                || (oppositeColor == PieceColor.Black ? char.IsLower(cells[row, col]) : char.IsUpper(cells[row, col]))
                );
            if (CellIsValid(king.Row - 1, king.Col - 1) && canBeMovedTo(king.Row - 1, king.Col - 1))
            {
                return true;
            }
            if (CellIsValid(king.Row - 1, king.Col) && canBeMovedTo(king.Row - 1, king.Col))
            {
                return true;
            }
            if (CellIsValid(king.Row - 1, king.Col + 1) && canBeMovedTo(king.Row - 1, king.Col + 1))
            {
                return true;
            }
            if (CellIsValid(king.Row, king.Col - 1) && canBeMovedTo(king.Row, king.Col - 1))
            {
                return true;
            }
            if (CellIsValid(king.Row, king.Col + 1) && canBeMovedTo(king.Row, king.Col + 1))
            {
                return true;
            }
            if (CellIsValid(king.Row + 1, king.Col - 1) && canBeMovedTo(king.Row + 1, king.Col - 1))
            {
                return true;
            }
            if (CellIsValid(king.Row + 1, king.Col) && canBeMovedTo(king.Row + 1, king.Col))
            {
                return true;
            }
            if (CellIsValid(king.Row + 1, king.Col + 1) && canBeMovedTo(king.Row + 1, king.Col + 1))
            {
                return true;
            }
            return false;
        }

        private static bool CheckIfThereAreMovesToStopCheck(PieceColor turnColor,
            List<(Piece, Piece)> defenders,
            List<(Piece, List<CellCoordinates>, CellCoordinates?)> attackers,
            char[,] cells,
            Dictionary<PieceColor, List<Piece>> pieces,
            CellCoordinates? enPassantCoordinates)
        {
            var attackerInterceptingCells = attackers.FirstOrDefault().Item2;
            var defendingPieces = defenders.Select(x => x.Item1).ToList();
            foreach (var pawn in pieces[turnColor].Where(p => p.PieceType == PieceType.Pawn
            && !defenders.Any(d => d.Item1.HasEqualCoordinates(p.Row, p.Col))))
            {
                var canStopCheck = LegalMoveFinder.HasLegalMoves(pawn,
                    cells,
                    enPassantCoordinates,
                    attackerInterceptingCells);
                if (canStopCheck)
                {
                    return true;
                }
            }
            if (attackers.FirstOrDefault().Item1.PieceType == PieceType.Pawn)
            {
                if (enPassantCoordinates.HasValue
                    && enPassantCoordinates.Value.RowDifference(attackerInterceptingCells.FirstOrDefault()) == 1)
                {
                    attackerInterceptingCells.Add(enPassantCoordinates.Value);
                }
            }
            foreach (var cell in attackerInterceptingCells)
            {
                var currAttackersAndInterceptingCells = AttackerFinder.GetAttackersAndInterceptingMoves(cell.Row,
                    cell.Col,
                    turnColor,
                    cells,
                    pieces);
                foreach (var attacker in currAttackersAndInterceptingCells.Select(a => a.Item1))
                {
                    if (enPassantCoordinates != null && cell.Equals(enPassantCoordinates.Value))
                    {
                        return true;
                    }
                    if (attacker.PieceType == PieceType.Pawn && cells[cell.Row, cell.Col] == '.')
                    {
                        continue;
                    }
                    if (defendingPieces.Any(p => p.HasEqualCoordinates(attacker.Row, attacker.Col)))
                    {
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
