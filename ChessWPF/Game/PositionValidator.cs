using ChessWPF.Models.Pieces.Enums;
using ChessWPF.Models.Positions;
using System;
using System.Linq;

namespace ChessWPF.Game
{
    public static class PositionValidator
    {
        private static Position position;

        public static bool ValidatePosition(Position positionInput)
        {
            position = positionInput;
            
            if (!ValidatePieceComposition())
            {
                return false;
            }
            
            if (!ValidateCastlingRights())
            {
                return false;
            }
            if (!(position.EnPassantCoordinates != null ? ValidateEnPassant() : true))
            {
                return false;
            }
            if (!ValidateOppositeTurnColorKingCannotBeTaken())
            {
                return false;
            }
            return true;
        }

        public static bool ValidatePositionFromFenAnnotation(string fenAnnotation)
        {
            position = FenAnnotationReader.GetPosition(fenAnnotation);
            var pieceCompositionIsCorrect = ValidatePieceComposition();
            var castlingIsValid = ValidateCastlingRights();
            var enPassantIsValid = position.EnPassantCoordinates != null ? ValidateEnPassant() : true;
            var oppositeTurnColorKingCannotBeTaken = ValidateOppositeTurnColorKingCannotBeTaken();
            return pieceCompositionIsCorrect
                && castlingIsValid 
                && enPassantIsValid
                && oppositeTurnColorKingCannotBeTaken;
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
            var enPassantRow = position.EnPassantCoordinates!.Value.Item1;
            var enPassantCol = position.EnPassantCoordinates.Value.Item2;

            if (enPassantRow == 2 && position.SimplifiedCells[enPassantRow, enPassantCol] == '.'
                && position.SimplifiedCells[enPassantRow + 1, enPassantCol] == 'p'
                && position.SimplifiedCells[enPassantRow -1, enPassantCol] == '.')
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
            if (!CheckRowsAndColumnsForAttackers(oppositeTurnColorKing!.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }
            if (!CheckDiagonalsForAttackers(oppositeTurnColorKing.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }
            if (!CheckForKnightAttackers(oppositeTurnColorKing.Row, oppositeTurnColorKing.Col, position.TurnColor))
            {
                return false;
            }

            return true;
        }

        private static bool CheckRowsAndColumnsForAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'R', 'O', 'Q', 'K' } : new char[] { 'r', 'o', 'q', 'k' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            var increment = 1;
            while (CellIsValid(row + increment, col))
            {
                if (char.IsLetter(cells[row + increment, col]))
                {
                    if (isAttacker(cells[row + increment, col]))
                    {
                        return false;
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
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row - increment, col]))
                    {
                        return false;
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
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row, col + increment]))
                    {
                        return false;
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
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row, col - increment]))
                    {
                        return false;
                    }
                    else
                    {
                        break;
                    }
                }
                increment++;
            }
            return true;
        }

        private static bool CheckDiagonalsForAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'P', 'B', 'Q', 'K' } : new char[] { 'p', 'b', 'q', 'k' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            var rowIncrement = 1;
            var colIncrement = 1;
            while (CellIsValid(row + rowIncrement, col + colIncrement))
            {
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row + rowIncrement, col + colIncrement]))
                    {
                        return false;
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
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row + rowIncrement, col - colIncrement]))
                    {
                        return false;
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
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row - rowIncrement, col + colIncrement]))
                    {
                        return false;
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
                if (char.IsLetter(cells[row, col]))
                {
                    if (isAttacker(cells[row - rowIncrement, col - colIncrement]))
                    {
                        return false;
                    }
                    else
                    {
                        break;
                    }
                }
                rowIncrement++;
                colIncrement++;
            }
            return true;
        }

        private static bool CheckForKnightAttackers(int row, int col, PieceColor attackerColor)
        {
            var cells = position.SimplifiedCells;
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'N', 'O' } : new char[] { 'n', 'n' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            if (CellIsValid(row + 2, col + 1) && isAttacker(cells[row + 2, col + 1]))
            {
                return false;
            }
            if (CellIsValid(row + 2, col - 1) && isAttacker(cells[row + 2, col - 1]))
            {
                return false;
            }
            if (CellIsValid(row - 2, col + 1) && isAttacker(cells[row - 2, col + 1]))
            {
                return false;
            }
            if (CellIsValid(row - 2, col - 1) && isAttacker(cells[row - 2, col - 1]))
            {
                return false;
            }

            if (CellIsValid(row + 1, col + 2) && isAttacker(cells[row + 1, col + 2]))
            {
                return false;
            }
            if (CellIsValid(row + 1, col - 2) && isAttacker(cells[row + 1, col - 2]))
            {
                return false;
            }
            if (CellIsValid(row - 1, col + 2) && isAttacker(cells[row - 1, col + 2]))
            {
                return false;
            }
            if (CellIsValid(row - 1, col - 2) && isAttacker(cells[row - 1, col - 2]))
            {
                return false;
            }
            return true;
        }

        private static bool CellIsValid(int row, int col)
        {
            return row > 0 && row < position.SimplifiedCells.GetLength(0) &&
                col > 0 && col < position.SimplifiedCells.GetLength(1);
        }


    }
}
