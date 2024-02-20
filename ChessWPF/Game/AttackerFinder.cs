using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Game
{
    public static class AttackerFinder
    {
        private static char[,] cells;
        private static Dictionary<PieceColor, List<Piece>> pieces;

        public static bool HasAttackers(int row, int col,
            PieceColor attackerColor,
            char[,] simplifiedCells)
        {
            cells = simplifiedCells;

            if (CheckRowsAndColumnsForAttackers(row, col, attackerColor))
            {
                return true;
            }
            if (CheckDiagonalsForAttackers(row, col, attackerColor))
            {
                return true;
            }
            if (CheckForKnightAttackers(row, col, attackerColor))
            {
                return true;
            }
            if (CheckForPawnAttackers(row, col, attackerColor))
            {
                return true;
            }
            if (CheckForKingAttackers(row, col, attackerColor))
            {
                return true;
            }
            return false;
        }

        public static List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>> GetAttackersAndInterceptingMoves(
            int row, int col,
            PieceColor attackerColor,
            char[,] simplifiedCells,
            Dictionary<PieceColor, List<Piece>> piecesInput)
        {
            cells = simplifiedCells;
            pieces = piecesInput;
            var attackers = new List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>>();
            GetRowAndColumnAttackers(row, col, attackerColor, attackers);
            GetDiagonalAttackers(row, col, attackerColor, attackers);
            GetKnightAttackers(row, col, attackerColor, attackers);
            GetPawnAttackers(row, col, attackerColor, attackers);
            return attackers;
        }

        private static bool CheckRowsAndColumnsForAttackers(int row, int col, PieceColor attackerColor)
        {
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

        private static void GetRowAndColumnAttackers(
            int row,
            int col,
            PieceColor attackerColor,
            List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>> attackers)
        {
            var interceptingCells = new List<CellCoordinates>();
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'R', 'O', 'Q' } : new char[] { 'r', 'o', 'q' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            var increment = 1;
            while (CellIsValid(row + increment, col))
            {
                interceptingCells.Add(new CellCoordinates(row + increment, col));
                if (char.IsLetter(cells[row + increment, col]))
                {
                    if (isAttacker(cells[row + increment, col]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row + increment, col))!,
                            new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row - 1, col))
                        {
                            var xRayAttackedCell = new CellCoordinates(row - 1, col);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;

                }
                increment++;
            }
            increment = 1;
            interceptingCells.Clear();

            while (CellIsValid(row - increment, col))
            {
                interceptingCells.Add(new CellCoordinates(row - increment, col));
                if (char.IsLetter(cells[row - increment, col]))
                {
                    if (isAttacker(cells[row - increment, col]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row - increment, col))!,
                            new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row + 1, col))
                        {
                            var xRayAttackedCell = new CellCoordinates(row + 1, col);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            interceptingCells.Clear();

            while (CellIsValid(row, col + increment))
            {
                interceptingCells.Add(new CellCoordinates(row, col + increment));
                if (char.IsLetter(cells[row, col + increment]))
                {
                    if (isAttacker(cells[row, col + increment]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row, col + increment))!,
                            new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row, col - 1))
                        {
                            var xRayAttackedCell = new CellCoordinates(row, col - 1);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            interceptingCells.Clear();
            while (CellIsValid(row, col - increment))
            {
                interceptingCells.Add(new CellCoordinates(row, col - increment));
                if (char.IsLetter(cells[row, col - increment]))
                {
                    if (isAttacker(cells[row, col - increment]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row, col - increment))!,
                                new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row, col + 1))
                        {
                            var xRayAttackedCell = new CellCoordinates(row, col + 1);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;
            }

        }

        private static bool CheckDiagonalsForAttackers(int row, int col, PieceColor attackerColor)
        {
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'B', 'Q' } : new char[] { 'b', 'q' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            var increment = 1;
            while (CellIsValid(row + increment, col + increment))
            {
                if (char.IsLetter(cells[row + increment, col + increment]))
                {
                    if (isAttacker(cells[row + increment, col + increment]))
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
            while (CellIsValid(row + increment, col - increment))
            {
                if (char.IsLetter(cells[row + increment, col - increment]))
                {
                    if (isAttacker(cells[row + increment, col - increment]))
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
            while (CellIsValid(row - increment, col + increment))
            {
                if (char.IsLetter(cells[row - increment, col + increment]))
                {
                    if (isAttacker(cells[row - increment, col + increment]))
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
            while (CellIsValid(row - increment, col - increment))
            {
                if (char.IsLetter(cells[row - increment, col - increment]))
                {
                    if (isAttacker(cells[row - increment, col - increment]))
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

        private static void GetDiagonalAttackers(int row, int col, PieceColor attackerColor,
            List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>> attackers)
        {
            var interceptingCells = new List<CellCoordinates>();
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'B', 'Q' } : new char[] { 'b', 'q' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);

            var increment = 1;
            while (CellIsValid(row + increment, col + increment))
            {
                interceptingCells.Add(new CellCoordinates(row + increment, col + increment));
                if (char.IsLetter(cells[row + increment, col + increment]))
                {
                    if (isAttacker(cells[row + increment, col + increment]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row + increment, col + increment))!,
                            new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row - 1, col - 1))
                        {
                            var xRayAttackedCell = new CellCoordinates(row - 1, col - 1);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;

            }
            increment = 1;
            interceptingCells.Clear();
            while (CellIsValid(row + increment, col - increment))
            {
                interceptingCells.Add(new CellCoordinates(row + increment, col - increment));
                if (char.IsLetter(cells[row + increment, col - increment]))
                {
                    if (isAttacker(cells[row + increment, col - increment]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row + increment, col - increment))!,
                            new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row - 1, col + 1))
                        {
                            var xRayAttackedCell = new CellCoordinates(row - 1, col + 1);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            interceptingCells.Clear();
            while (CellIsValid(row - increment, col + increment))
            {
                interceptingCells.Add(new CellCoordinates(row - increment, col + increment));
                if (char.IsLetter(cells[row - increment, col + increment]))
                {
                    if (isAttacker(cells[row - increment, col + increment]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                            .FirstOrDefault(p => p.HasEqualCoordinates(row - increment, col + increment))!,
                            new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row + 1, col - 1))
                        {
                            var xRayAttackedCell = new CellCoordinates(row + 1, col - 1);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;
            }
            increment = 1;
            interceptingCells.Clear();
            while (CellIsValid(row - increment, col - increment))
            {
                interceptingCells.Add(new CellCoordinates(row - increment, col - increment));
                if (char.IsLetter(cells[row - increment, col - increment]))
                {
                    if (isAttacker(cells[row - increment, col - increment]))
                    {
                        var attacker = new ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>(pieces[attackerColor]
                             .FirstOrDefault(p => p.HasEqualCoordinates(row - increment, col - increment))!,
                             new List<CellCoordinates>(interceptingCells), null);
                        if (CellIsValid(row + 1, col + 1))
                        {
                            var xRayAttackedCell = new CellCoordinates(row + 1, col + 1);
                            attacker.Item3 = xRayAttackedCell;
                        }
                        attackers.Add(attacker);
                    }
                    break;
                }
                increment++;
            }
        }

        private static bool CheckForKnightAttackers(int row, int col, PieceColor attackerColor)
        {
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

        private static void GetKnightAttackers(int row,
            int col,
            PieceColor attackerColor,
            List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>> attackers)
        {

            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'N', 'O' } : new char[] { 'n', 'o' };
            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            if (CellIsValid(row + 2, col + 1) && isAttacker(cells[row + 2, col + 1]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row + 2, col + 1))!,
                    new List<CellCoordinates>()
                    {
                        new CellCoordinates(row + 2, col +1)
                    },
                    null));
            }
            if (CellIsValid(row + 2, col - 1) && isAttacker(cells[row + 2, col - 1]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row + 2, col - 1))!,
                    new List<CellCoordinates>()
                    {
                        new CellCoordinates(row + 2, col -1)
                    },
                    null));
            }
            if (CellIsValid(row - 2, col + 1) && isAttacker(cells[row - 2, col + 1]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row - 2, col + 1))!,
                    new List<CellCoordinates>()
                    {
                        new CellCoordinates(row - 2, col + 1)
                    },
                    null));
            }
            if (CellIsValid(row - 2, col - 1) && isAttacker(cells[row - 2, col - 1]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row - 2, col - 1))!,
                    new List<CellCoordinates>()
                    {
                        new CellCoordinates(row - 2, col - 1)
                    },
                    null));
            }
            if (CellIsValid(row + 1, col + 2) && isAttacker(cells[row + 1, col + 2]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row + 1, col + 2))!,
                  new List<CellCoordinates>()
                  {
                        new CellCoordinates(row + 1, col + 2)
                  },
                  null));
            }
            if (CellIsValid(row + 1, col - 2) && isAttacker(cells[row + 1, col - 2]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row + 1, col - 2))!,
                  new List<CellCoordinates>()
                  {
                        new CellCoordinates(row + 1, col - 2)
                  },
                  null));
            }
            if (CellIsValid(row - 1, col + 2) && isAttacker(cells[row - 1, col + 2]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row - 1, col + 2))!,
                  new List<CellCoordinates>()
                  {
                        new CellCoordinates(row - 1, col + 2)
                  },
                  null));
            }
            if (CellIsValid(row - 1, col - 2) && isAttacker(cells[row - 1, col - 2]))
            {
                attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row - 1, col - 2))!,
                  new List<CellCoordinates>()
                  {
                        new CellCoordinates(row - 1, col - 2)
                  },
                  null));
            }
        }

        private static bool CheckForPawnAttackers(int row, int col, PieceColor attackerColor)
        {
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

        private static void GetPawnAttackers(int row, int col, PieceColor attackerColor,
            List<ValueTuple<Piece, List<CellCoordinates>, CellCoordinates?>> attackers)
        {
            var attackerTypes = attackerColor == PieceColor.White ? new char[] { 'P' } : new char[] { 'p' };

            Predicate<char> isAttacker = (attacker) => attackerTypes.Contains(attacker);
            if (attackerColor == PieceColor.White)
            {
                if (CellIsValid(row + 1, col - 1) && isAttacker(cells[row + 1, col - 1]))
                {
                    attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row + 1, col - 1))!, new List<CellCoordinates>()
                    {
                        new CellCoordinates(row + 1 , col - 1)
                    }
                    ,
                    null));
                }
                if (CellIsValid(row + 1, col + 1) && isAttacker(cells[row + 1, col + 1]))
                {
                    attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row + 1, col + 1))!, new List<CellCoordinates>()
                    {
                        new CellCoordinates(row + 1 , col + 1)
                    },
                    null));
                }
            }
            else
            {
                if (CellIsValid(row - 1, col - 1) && isAttacker(cells[row - 1, col - 1]))
                {
                    attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row - 1, col - 1))!, new List<CellCoordinates>()
                    {
                        new CellCoordinates(row - 1 , col - 1)
                    },
                    null));
                }
                if (CellIsValid(row - 1, col + 1) && isAttacker(cells[row - 1, col + 1]))
                {
                    attackers.Add((pieces[attackerColor].FirstOrDefault(p => p.HasEqualCoordinates(row - 1, col + 1))!, new List<CellCoordinates>()
                    {
                        new CellCoordinates(row - 1 , col + 1)
                    },
                    null));
                }
            }
        }

        private static bool CheckForKingAttackers(int row, int col, PieceColor attackerColor)
        {
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
            if (CellIsValid(row + 1, col - 1) && isAttacker(cells[row + 1, col - 1]))
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


        private static bool CellIsValid(int row, int col)
        {
            return row >= 0 && row < cells.GetLength(0) &&
                col >= 0 && col < cells.GetLength(1);
        }
    }
}
