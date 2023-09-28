using ChessWPF.Constants;
using ChessWPF.Game;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Models.Data.Board
{
    public class Board
    {
        private string gameResult;
        private string fenAnnotation;
        private bool gameHasStarted;
        private bool gameHasEnded;
        private int halfMoveCount;
        private Cell[,] cells;
        private PieceColor turnColor;
        private Stack<Move> moves;
        private Dictionary<PieceColor, List<Piece>> pieces;
        private Move undoneMove;
        private Move promotionMove;
        private List<Cell> backupCells;

        public Board()
        {
            Cells = new Cell[8, 8];
            Moves = new Stack<Move>();
            Pieces = new Dictionary<PieceColor, List<Piece>>
            {
                { PieceColor.White, new List<Piece>() },
                { PieceColor.Black, new List<Piece>() }
            };
            CreateCells(Cells);
            BackupCells = new List<Cell>();
            SetupPieces();

            //SetupPiecesDemo();
            //SetupPiecesCheckBishopTest();
            //SetupPiecesCheckRookTest();
            //SetupPiecesEnPassantTest();
            //SetupPiecesPromotionTest();
            //SetupPiecesStalemateTest();
            //SetupPiecesKnightCheckTest();
            //SetupPiecesKnookTest();
        }


        public string GameResult
        {
            get => gameResult;
            set => gameResult = value;
        }

        public string FenAnnotation
        {
            get => fenAnnotation;
        }

        public bool GameHasStarted
        {
            get { return gameHasStarted; }
            set { gameHasStarted = value; }
        }
        public bool GameHasEnded
        {
            get { return gameHasEnded; }
            set { gameHasEnded = value; }
        }

        public int HalfMoveCount
        {
            get { return halfMoveCount; }
        }
        public Cell[,] Cells
        {
            get => cells;
            set => cells = value;
        }
        public PieceColor TurnColor
        {
            get => turnColor;
            set => turnColor = value;
        }
        public Stack<Move> Moves
        {
            get => moves;
            set => moves = value;
        }

        public Dictionary<PieceColor, List<Piece>> Pieces
        {
            get => pieces;
            set => pieces = value;
        }

        public Move UndoneMove
        {
            get => undoneMove;
            set => undoneMove = value;
        }

        public Move PromotionMove
        {
            get => promotionMove;
            set => promotionMove = value;
        }

        public List<Cell> BackupCells
        {
            get => backupCells;
            set => backupCells = value;
        }

        public void CreateCells(Cell[,] cells)
        {
            for (int row = 0; row < cells.GetLength(0); row++)
            {
                for (int col = 0; col < cells.GetLength(1); col++)
                {
                    cells[row, col] = new Cell(row, col);
                }
            }
        }

        public void ReverseTurnColor()
        {
            if (TurnColor == PieceColor.White)
            {
                TurnColor = PieceColor.Black;
            }
            else if (TurnColor == PieceColor.Black)
            {
                TurnColor = PieceColor.White;
            }
        }

        public void UpdateFenAnnotation()
        {
            fenAnnotation = FenAnnotationWriter.WriteFenAnnotation(this);
        }

        public Move MovePiece(Cell cell, Cell selectedCell)
        {
            var selectedPieceType = selectedCell.Piece.PieceType;
            Move move = CreateMove(cell, selectedCell);
            if (move.CellTwoAfter.Piece.PieceType == PieceType.King && Math.Abs(move.CellOneBefore.Col - move.CellTwoAfter.Col) == 2)
            {
                move = MoveRookInCastlingMove(move);
            }
            else if (selectedPieceType == PieceType.Pawn)
            {
                if (move.CellTwoAfter.Piece.PieceType == PieceType.Pawn
                    && (move.CellTwoAfter.Row == 0 || move.CellTwoAfter.Row == 7))
                {
                    var pawn = move.CellTwoAfter.Piece;
                    CreatePromotionMove(move, pawn);
                }
                if (Moves.Count > 0)
                {
                    var lastMove = Moves.Peek();
                    if (Math.Abs(lastMove.CellOneBefore.Row - lastMove.CellTwoBefore.Row) == 2)
                    {
                        var lastMovedPiece = lastMove.CellTwoAfter.Piece;
                        if (lastMovedPiece.PieceType == PieceType.Pawn
                            && cell.Col == move.CellTwoAfter.Piece.Cell.Col
                            && move.CellTwoBefore.Piece == null
                            && Math.Abs(lastMovedPiece.Cell.Col - move.CellOneBefore.Col) == 1
                            && lastMovedPiece.Cell.Col == move.CellTwoBefore.Col
                            && lastMovedPiece.Cell.Row == move.CellOneBefore.Row)
                        {
                            move = EnPassantMove(move);
                        }
                    }
                }
            }

            return move;
        }

        public void UndoMove()
        {
            Move move = Moves.Pop();
            Cell cellOneBefore = move.CellOneBefore;
            Cells[cellOneBefore.Row, cellOneBefore.Col] = cellOneBefore;
            Cells[cellOneBefore.Row, cellOneBefore.Col].Piece = cellOneBefore.Piece;

            Cell cellTwoBefore = move.CellTwoBefore;
            Cells[cellTwoBefore.Row, cellTwoBefore.Col] = cellTwoBefore;
            Cells[cellTwoBefore.Row, cellTwoBefore.Col].Piece = cellTwoBefore.Piece;

            if (move.CellThreeBefore != null)
            {
                Cell cellThreeBefore = move.CellThreeBefore;
                Cells[cellThreeBefore.Row, cellThreeBefore.Col] = cellThreeBefore;
                Cells[cellThreeBefore.Row, cellThreeBefore.Col].Piece = cellThreeBefore.Piece;
            }

            if (move.CellFourBefore != null)
            {
                Cell cellFourBefore = move.CellFourBefore;
                Cells[cellFourBefore.Row, cellFourBefore.Col] = cellFourBefore;
                Cells[cellFourBefore.Row, cellFourBefore.Col].Piece = cellFourBefore.Piece;
            }


            UndoneMove = move;
            ReverseTurnColor();
            if (moves.Any())
            {
                var lastMove = moves.Peek();
                fenAnnotation = lastMove.FenAnnotation;
                if (lastMove.IsHalfMove())
                {
                    halfMoveCount = lastMove.CurrHalfMoveCount;
                }
            }
            else
            {
                UpdateFenAnnotation();
                halfMoveCount = 0;
            }
        }

        public void UndoPromotionMove()
        {
            Cells[PromotionMove.CellOneBefore.Row, PromotionMove.CellOneBefore.Col] = PromotionMove.CellOneBefore;
            Cells[PromotionMove.CellOneBefore.Row, PromotionMove.CellOneBefore.Col].Piece = PromotionMove.CellOneBefore.Piece;

            Cells[PromotionMove.CellTwoBefore.Row, PromotionMove.CellTwoBefore.Col] = PromotionMove.CellTwoBefore;
            if (PromotionMove.CellTwoBefore.Piece != null)
            {
                Cells[PromotionMove.CellTwoBefore.Row, PromotionMove.CellTwoBefore.Col].Piece = PromotionMove.CellTwoBefore.Piece;
            }

        }

        public void PromotePiece(PieceType pieceType)
        {
            var cell = promotionMove.CellTwoAfter;
            cell.Piece = PieceConstructor.ConstructPieceByType(pieceType, TurnColor, cell);
            promotionMove.CellTwoAfter.Piece = cell.Piece;
            promotionMove.CellTwoAfter.Piece.Cell = cell;
            Cells[cell.Row, cell.Col].Piece = cell.Piece;
            Cells[cell.Row, cell.Col].Piece.Cell = cell;
        }

        public void CalculatePossibleMoves()
        {
            var king = (King)Pieces[TurnColor].First(p => p.PieceType == PieceType.King);
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            king.Attackers.Clear();
            king.Defenders = KingDefenderFinder.FindDefenders(king, TurnColor);
            (Cells[king.Cell.Row, king.Cell.Col].Piece as King).IsInCheck = false;

            foreach (var piece in Pieces[oppositeColor])
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece);
                piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                var checkedKingCell = piece.LegalMoves.FirstOrDefault(c => c.Piece != null && c.Piece.PieceType == PieceType.King);
                if (checkedKingCell != null)
                {
                    king.Attackers.Add(piece);
                    king.IsInCheck = true;
                }
            }
            var validMovesToStopCheck = new List<Cell>();
            if (king.Attackers.Count == 1)
            {
                validMovesToStopCheck = CheckDirectionFinder.GetLegalMovesToStopCheck(king, king.Attackers.First(), this);
            }
            foreach (var piece in Pieces[TurnColor])
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece);
                if (validMovesToStopCheck.Count > 0 && piece.PieceType != PieceType.King && !king.Defenders.Any(d => d.Item1 == piece))
                {
                    legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]
                        .Where(lm => validMovesToStopCheck.Contains(lm)).ToList();
                }
                else if (king.Defenders.Any(d => d.Item1 == piece))
                {
                    if (king.IsInCheck)
                    {
                        legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = new List<Cell>();
                    }
                    var currDefenderAndPotentialAttacker = king.Defenders.First(d => d.Item1 == piece);
                    var movesToPreventPotentialCheck = CheckDirectionFinder.GetLegalMovesToStopCheck(king, currDefenderAndPotentialAttacker.Item2, this);
                    movesToPreventPotentialCheck.Remove(currDefenderAndPotentialAttacker.Item1.Cell);
                    movesToPreventPotentialCheck.Add(currDefenderAndPotentialAttacker.Item2.Cell);
                    if (king.Attackers.Count == 1 && king.Attackers.First().PieceType == PieceType.Knight)
                    {
                        legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = new List<Cell>();
                    }
                    legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Where(lm => movesToPreventPotentialCheck.Contains(lm)).ToList();
                }
                piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
            }
        }

        public bool CheckForGameEnding()
        {
            var king = (King)Pieces[TurnColor].First(p => p.PieceType == PieceType.King);
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            if (Pieces[TurnColor].Sum(p => p.LegalMoves.Count) == 0)
            {
                if (king.Attackers.Count > 0)
                {
                    GameResult = $"{oppositeColor} wins by checkmate!";
                    return true;
                }
                else
                {
                    GameResult = "Stalemate!";
                    return true;

                }
            }
            if (CheckForDraw())
            {
                return true;
            }
            else if (Moves.Count > 9)
            {
                if (CheckForThreefoldRepetition())
                {
                    return true;
                }
            }
            return false;
        }

        public void FinishMove(Move move, Cell selectedCell)
        {
            if (selectedCell != null)
            {
                selectedCell.Piece = null;
            }
            if (move.IsHalfMove())
            {
                halfMoveCount++;
            }
            else
            {
                halfMoveCount = 0;
            }
            move.CurrHalfMoveCount = halfMoveCount;
            this.ReverseTurnColor();
            Moves.Push(move);
            UpdateFenAnnotation();
            Moves.Peek().FenAnnotation = this.fenAnnotation;
        }

        public Move CreateMove(Cell cell, Cell selectedCell)
        {
            Move move = new Move();
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            move.CellOneBefore = new Cell(selectedCell.Row, selectedCell.Col);
            move.CellOneBefore.Piece = PieceConstructor.ConstructPieceByType(selectedCell.Piece.PieceType, TurnColor, move.CellOneBefore);
            move.CellTwoBefore = new Cell(cell.Row, cell.Col, cell.Piece);
            if (cell.Piece != null)
            {
                move.CellTwoBefore.Piece = PieceConstructor.ConstructPieceByType(cell.Piece.PieceType, oppositeColor, move.CellTwoBefore);
            }

            Cells[cell.Row, cell.Col].Piece = selectedCell.Piece;

            Cells[cell.Row, cell.Col].Piece.Cell = new Cell(cell.Row, cell.Col, Cells[cell.Row, cell.Col].Piece);

            move.CellOneAfter = new Cell(selectedCell.Row, selectedCell.Col, null);

            move.CellTwoAfter = new Cell(cell.Row, cell.Col);
            move.CellTwoAfter.Piece = PieceConstructor.ConstructPieceByType(cell.Piece.PieceType, TurnColor, move.CellTwoAfter);
            selectedCell.Piece = null;

            return move;
        }

        public Move EnPassantMove(Move move)
        {
            var pawn = move.CellTwoAfter.Piece;
            switch (pawn.Color)
            {
                case PieceColor.White:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Cells[move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col].Piece.PieceType, PieceColor.Black, move.CellThreeBefore);
                    break;
                case PieceColor.Black:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Cells[move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col].Piece.PieceType, PieceColor.White, move.CellThreeBefore);
                    break;
            }
            Cells[move.CellThreeBefore.Row, move.CellThreeBefore.Col].Piece = null;
            move.CellThreeAfter = new Cell(move.CellThreeBefore.Row, move.CellThreeBefore.Col, null);
            return move;
        }

        public Move MoveRookInCastlingMove(Move move)
        {
            var king = move.CellTwoAfter.Piece;
            var colDiff = move.CellOneBefore.Col - move.CellTwoBefore.Col;
            var cell = new Cell(0, 0);
            var selectedCell = new Cell(-1, -1);
            var rookMove = new Move();
            switch (king.Color)
            {
                case PieceColor.White:
                    if (colDiff < 0)
                    {
                        cell = Cells[king.Cell.Row, king.Cell.Col - 1];
                        selectedCell = Cells[king.Cell.Row, king.Cell.Col + 1];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    if (colDiff > 0)
                    {
                        cell = Cells[king.Cell.Row, king.Cell.Col + 1];
                        selectedCell = Cells[king.Cell.Row, king.Cell.Col - 2];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    break;
                case PieceColor.Black:
                    if (colDiff < 0)
                    {
                        cell = Cells[king.Cell.Row, king.Cell.Col - 1];
                        selectedCell = Cells[king.Cell.Row, king.Cell.Col + 1];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    if (colDiff > 0)
                    {
                        cell = Cells[king.Cell.Row, king.Cell.Col + 1];
                        selectedCell = Cells[king.Cell.Row, king.Cell.Col - 2];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    break;
            }
            move.CellThreeBefore = rookMove.CellOneBefore;
            move.CellThreeAfter = rookMove.CellOneAfter;
            move.CellFourBefore = rookMove.CellTwoBefore;
            move.CellFourAfter = rookMove.CellTwoAfter;
            return move;
        }

        public void CreatePromotionMove(Move move, Piece pawn)
        {
            BackupCells = new List<Cell>();
            var promotionCellsPieces = new List<PieceType>() { PieceType.Queen, PieceType.Knight, PieceType.Rook, PieceType.Bishop, PieceType.Knook };

            var rowIncrement = 0;
            switch (pawn.Color)
            {
                case PieceColor.White:
                    for (int i = 0; i < 5; i++)
                    {
                        var currCell = new Cell(pawn.Cell.Row + rowIncrement++, pawn.Cell.Col);
                        if (currCell.HasEqualRowAndCol(move.CellTwoBefore))
                        {
                            BackupCells.Add(currCell);
                            if (move.CellTwoBefore.Piece != null)
                            {
                                currCell.Piece = PieceConstructor.ConstructPieceByType(move.CellTwoBefore.Piece.PieceType, move.CellTwoBefore.Piece.Color, currCell);
                                BackupCells[i].Piece = currCell.Piece;
                            }
                        }
                        else
                        {
                            BackupCells.Add(currCell);

                            if (Cells[currCell.Row, currCell.Col].Piece != null)
                            {
                                BackupCells[i].Piece = PieceConstructor.ConstructPieceByType(Cells[currCell.Row, currCell.Col].Piece.PieceType, Cells[currCell.Row, currCell.Col].Piece.Color, Cells[currCell.Row, currCell.Col]);
                            }
                        }
                        Cells[currCell.Row, currCell.Col].Piece = PieceConstructor.ConstructPieceForPromotion(promotionCellsPieces[i], pawn.Color);
                    }
                    break;
                case PieceColor.Black:
                    for (int i = 0; i < 5; i++)
                    {
                        var currCell = new Cell(pawn.Cell.Row - rowIncrement++, pawn.Cell.Col);
                        if (currCell.HasEqualRowAndCol(move.CellTwoBefore))
                        {
                            BackupCells.Add(currCell);
                            if (move.CellTwoBefore.Piece != null)
                            {
                                currCell.Piece = PieceConstructor.ConstructPieceByType(move.CellTwoBefore.Piece.PieceType, move.CellTwoBefore.Piece.Color, currCell);
                                BackupCells[i].Piece = currCell.Piece;
                            }
                        }
                        else
                        {
                            BackupCells.Add(currCell);

                            if (Cells[currCell.Row, currCell.Col].Piece != null)
                            {
                                BackupCells[i].Piece = PieceConstructor.ConstructPieceByType(Cells[currCell.Row, currCell.Col].Piece.PieceType, Cells[currCell.Row, currCell.Col].Piece.Color, Cells[currCell.Row, currCell.Col]);
                            }
                        }
                        Cells[currCell.Row, currCell.Col].Piece = PieceConstructor.ConstructPieceForPromotion(promotionCellsPieces[i], pawn.Color);
                    }
                    break;
            }
            PromotionMove = move;
        }

        public void RestoreBackupCells()
        {
            foreach (var cell in BackupCells.Where(c => c.Row != 7 && c.Row != 0))
            {
                Cells[cell.Row, cell.Col] = cell;
                if (cell.Piece != null)
                {
                    Cells[cell.Row, cell.Col].Piece = cell.Piece;
                }
                else
                {
                    Cells[cell.Row, cell.Col].Piece = null;
                }
            }
            BackupCells.Clear();
        }
        public void RestoreAllBackupCells()
        {
            foreach (var cell in BackupCells)
            {
                Cells[cell.Row, cell.Col] = cell;
                if (cell.Piece != null)
                {
                    Cells[cell.Row, cell.Col].Piece = cell.Piece;
                }
                else
                {
                    Cells[cell.Row, cell.Col].Piece = null;
                }
            }
            BackupCells.Clear();
        }

        private void SetupPieces()
        {
            SetupKings();
            SetupPawns();
            SetupQueens();
            SetupRooks();
            SetupBishops();
            SetupKnights();
        }

        private void SetupKings()
        {
            Cells[0, 4].Piece = new King(PieceColor.Black, Cells[0, 4]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
        }

        private void SetupQueens()
        {
            Cells[0, 3].Piece = new Queen(PieceColor.Black, Cells[0, 3]);

            Cells[7, 3].Piece = new Queen(PieceColor.White, Cells[7, 3]);
        }

        private void SetupBishops()
        {
            Cells[0, 2].Piece = new Bishop(PieceColor.Black, Cells[0, 2]);
            Cells[0, 5].Piece = new Bishop(PieceColor.Black, Cells[0, 5]);

            Cells[7, 2].Piece = new Bishop(PieceColor.White, Cells[7, 2]);
            Cells[7, 5].Piece = new Bishop(PieceColor.White, Cells[7, 5]);
        }

        private void SetupKnights()
        {
            Cells[0, 1].Piece = new Knight(PieceColor.Black, Cells[0, 1]);
            Cells[0, 6].Piece = new Knight(PieceColor.Black, Cells[0, 6]);

            Cells[7, 1].Piece = new Knight(PieceColor.White, Cells[7, 1]);
            Cells[7, 6].Piece = new Knight(PieceColor.White, Cells[7, 6]);
        }

        private void SetupPawns()
        {
            int row = 1;
            for (int col = 0; col < Cells.GetLength(0); col++)
            {
                Cells[row, col].Piece = new Pawn(PieceColor.Black, Cells[row, col]);
            }
            row = 6;
            for (int col = 0; col < Cells.GetLength(0); col++)
            {
                Cells[row, col].Piece = new Pawn(PieceColor.White, Cells[row, col]);
            }
        }

        private void SetupRooks()
        {
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);
            Cells[0, 7].Piece = new Rook(PieceColor.Black, Cells[0, 7]);

            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[7, 0].Piece = new Rook(PieceColor.White, Cells[7, 0]);
        }

        private void SetupPiecesDemo()
        {
            Cells[6, 3].Piece = new Pawn(PieceColor.White, Cells[6, 3]);
            Cells[6, 4].Piece = new Pawn(PieceColor.White, Cells[6, 4]);

            Cells[1, 3].Piece = new Pawn(PieceColor.Black, Cells[1, 3]);
            Cells[1, 4].Piece = new Pawn(PieceColor.Black, Cells[1, 4]);

            Cells[6, 6].Piece = new Bishop(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new Bishop(PieceColor.Black, Cells[1, 6]);

            Cells[4, 4].Piece = new Knight(PieceColor.White, Cells[4, 4]);
            Cells[3, 3].Piece = new Knight(PieceColor.Black, Cells[3, 3]);

            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);

            Cells[6, 2].Piece = new Queen(PieceColor.White, Cells[6, 2]);
            Cells[3, 4].Piece = new Queen(PieceColor.Black, Cells[3, 4]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[0, 4].Piece = new King(PieceColor.Black, Cells[0, 4]);

        }

        private void SetupPiecesCheckBishopTest()
        {
            Cells[6, 3].Piece = new Bishop(PieceColor.White, Cells[6, 3]);
            Cells[0, 2].Piece = new Bishop(PieceColor.Black, Cells[0, 2]);

            Cells[6, 6].Piece = new King(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new King(PieceColor.Black, Cells[1, 6]);
        }

        private void SetupPiecesCheckRookTest()
        {
            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);

            Cells[6, 6].Piece = new King(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new King(PieceColor.Black, Cells[1, 6]);
        }
        private void SetupPiecesEnPassantTest()
        {
            Cells[3, 2].Piece = new Pawn(PieceColor.White, Cells[3, 2]);
            Cells[6, 6].Piece = new Pawn(PieceColor.White, Cells[6, 6]);

            Cells[1, 1].Piece = new Pawn(PieceColor.Black, Cells[1, 1]);
            Cells[4, 5].Piece = new Pawn(PieceColor.Black, Cells[4, 5]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[1, 4].Piece = new King(PieceColor.Black, Cells[1, 4]);
        }

        private void SetupPiecesPromotionTest()
        {
            Cells[1, 1].Piece = new Pawn(PieceColor.White, Cells[1, 1]);
            Cells[1, 6].Piece = new Pawn(PieceColor.White, Cells[1, 6]);

            Cells[6, 1].Piece = new Pawn(PieceColor.Black, Cells[6, 1]);
            Cells[6, 7].Piece = new Pawn(PieceColor.Black, Cells[6, 7]);

            Cells[0, 5].Piece = new Queen(PieceColor.Black, Cells[0, 5]);
            Cells[0, 6].Piece = new Queen(PieceColor.Black, Cells[0, 6]);



            Cells[2, 7].Piece = new King(PieceColor.White, Cells[2, 7]);
            Cells[1, 5].Piece = new King(PieceColor.Black, Cells[1, 5]);
        }

        private void SetupPiecesStalemateTest()
        {
            Cells[6, 7].Piece = new Pawn(PieceColor.Black, Cells[6, 7]);

            //Cells[2, 6].Piece = new Queen(PieceColor.White, Cells[2, 6]);
            Cells[7, 7].Piece = new Bishop(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Bishop(PieceColor.Black, Cells[0, 0]);
            //Cells[6, 2].Piece = new Knight(PieceColor.Black, Cells[6, 2]);

            Cells[6, 6].Piece = new King(PieceColor.White, Cells[6, 6]);
            Cells[1, 4].Piece = new King(PieceColor.Black, Cells[1, 4]);
        }

        private void SetupPiecesKnightCheckTest()
        {
            Cells[6, 3].Piece = new Knight(PieceColor.White, Cells[6, 3]);
            Cells[7, 5].Piece = new Queen(PieceColor.White, Cells[7, 5]);

            Cells[3, 5].Piece = new Pawn(PieceColor.Black, Cells[3, 5]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[2, 5].Piece = new King(PieceColor.Black, Cells[2, 5]);
        }

        private void SetupPiecesKnookTest()
        {
            Cells[6, 3].Piece = new Pawn(PieceColor.White, Cells[6, 3]);
            Cells[6, 4].Piece = new Pawn(PieceColor.White, Cells[6, 4]);

            Cells[1, 3].Piece = new Pawn(PieceColor.Black, Cells[1, 3]);
            Cells[1, 4].Piece = new Pawn(PieceColor.Black, Cells[1, 4]);

            Cells[6, 6].Piece = new Bishop(PieceColor.White, Cells[6, 6]);
            Cells[1, 6].Piece = new Bishop(PieceColor.Black, Cells[1, 6]);

            Cells[4, 4].Piece = new Knight(PieceColor.White, Cells[4, 4]);
            Cells[3, 3].Piece = new Knight(PieceColor.Black, Cells[3, 3]);

            Cells[7, 7].Piece = new Rook(PieceColor.White, Cells[7, 7]);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, Cells[0, 0]);

            Cells[6, 2].Piece = new Knook(PieceColor.White, Cells[6, 2]);
            Cells[3, 4].Piece = new Knook(PieceColor.Black, Cells[3, 4]);

            Cells[7, 4].Piece = new King(PieceColor.White, Cells[7, 4]);
            Cells[0, 4].Piece = new King(PieceColor.Black, Cells[0, 4]);
        }

        private bool CheckForDraw()
        {
            var isGameDrawn = false;
            if (HalfMoveCount >= 100)
            {
                GameResult = "Draw! 100 moves were made with no pawn advances or piece captures!";
                isGameDrawn = true;
            }
            else if (Pieces.Sum(p => p.Value.Count) == 2)
            {
                GameResult = "Draw!";

                isGameDrawn = true;
            }
            else if (Pieces.Sum(p => p.Value.Count) == 3)
            {
                if (Pieces.Any(p => p.Value.Count == 2 && p.Value.Any(p => p.PieceType == PieceType.Bishop || p.PieceType == PieceType.Knight)))
                {
                    GameResult = "Draw! Insufficient pieces to checkmate!";
                    isGameDrawn = true;
                }
            }
            else if (Pieces.Sum(p => p.Value.Count) == 4)
            {
                if (Pieces.All(p => p.Value.Count == 2 && p.Value.Any(p => p.PieceType == PieceType.Bishop)))
                {
                    if ((Pieces[PieceColor.White].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell()
                        && Pieces[PieceColor.Black].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell())
                        || !Pieces[PieceColor.White].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell()
                        && !Pieces[PieceColor.Black].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell())
                    {
                        GameResult = "Draw! Insufficient pieces to checkmate!";
                        return true;
                    }
                }
            }

            return isGameDrawn;
        }

        private bool CheckForThreefoldRepetition()
        {
            var movesAreRepeated = false;

            var movesAsArray = Moves.Reverse().ToArray();
            if ((movesAsArray[movesAsArray.Length - 1].Equals(movesAsArray[movesAsArray.Length - 5])
                && movesAsArray[movesAsArray.Length - 1].IsOppositeMove(movesAsArray[movesAsArray.Length - 3]))
                && (movesAsArray[movesAsArray.Length - 3].Equals(movesAsArray[movesAsArray.Length - 7])
                && movesAsArray[movesAsArray.Length - 3].IsOppositeMove(movesAsArray[movesAsArray.Length - 5])
                && (movesAsArray[movesAsArray.Length - 5].Equals(movesAsArray[movesAsArray.Length - 9])
                && movesAsArray[movesAsArray.Length - 5].IsOppositeMove(movesAsArray[movesAsArray.Length - 7]))
                && (movesAsArray[movesAsArray.Length - 2].Equals(movesAsArray[movesAsArray.Length - 6])
                && movesAsArray[movesAsArray.Length - 2].IsOppositeMove(movesAsArray[movesAsArray.Length - 4]))
                && (movesAsArray[movesAsArray.Length - 4].Equals(movesAsArray[movesAsArray.Length - 8])
                && movesAsArray[movesAsArray.Length - 4].IsOppositeMove(movesAsArray[movesAsArray.Length - 6])))
                && (movesAsArray[movesAsArray.Length - 6].Equals(movesAsArray[movesAsArray.Length - 10])
                && movesAsArray[movesAsArray.Length - 6].IsOppositeMove(movesAsArray[movesAsArray.Length - 8])))
            {
                GameResult = "Draw! Threefold repetition!";
                movesAreRepeated = true;
            }
            return movesAreRepeated;
        }

    }
}
