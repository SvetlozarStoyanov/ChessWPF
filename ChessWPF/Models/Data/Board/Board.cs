﻿using ChessWPF.Constants;
using ChessWPF.Game;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Models.Data.Board
{
    public sealed class Board
    {
        private string gameResult;
        private string fenAnnotation;
        private bool gameHasStarted;
        private bool gameHasEnded;
        private int halfMoveCount;
        private PieceColor turnColor;
        private Move ongoingPromotionMove;
        private Stack<Move> moves;
        private Cell[,] cells;
        private List<Cell> backupCells;
        private Dictionary<PieceColor, List<Piece>> pieces;

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
            //TurnColor = PieceColor.White;

            SetupPieces();
            //SetupPiecesAnnotationTest();
            //SetupPiecesDemo();
            //SetupPiecesCheckBishopTest();
            //SetupPiecesCheckRookTest();
            //SetupPiecesEnPassantTest();
            //SetupPiecesEnPassantCheckSaveTest();
            //SetupPiecesPromotionTest();
            //SetupPiecesStalemateTest();
            //SetupPiecesKnightCheckTest();
            //SetupPiecesKnookTest();
            //SetupPiecesBeforeMoveCheckTest();
            //SetupPiecesPawnCheckTest();

            AddPiecesFromSetup();
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
            get => gameHasStarted;
            set { gameHasStarted = value; }
        }

        public bool GameHasEnded
        {
            get => gameHasEnded;
            set { gameHasEnded = value; }
        }

        public int HalfMoveCount
        {
            get => halfMoveCount;
        }

        public Move? OngoingPromotionMove
        {
            get => ongoingPromotionMove;
            private set => ongoingPromotionMove = value!;
        }

        public Cell[,] Cells
        {
            get => cells;
            private set => cells = value;
        }

        public PieceColor TurnColor
        {
            get => turnColor;
            private set => turnColor = value;
        }

        public Stack<Move> Moves
        {
            get => moves;
            private set => moves = value;
        }

        public List<Cell> BackupCells
        {
            get => backupCells;
            private set => backupCells = value;
        }

        public Dictionary<PieceColor, List<Piece>> Pieces
        {
            get => pieces;
            private set => pieces = value;
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

        public Move MovePiece(Cell movedToCell, Cell selectedCell)
        {
            var move = CreateMove(movedToCell, selectedCell);

            if (!move.IsPromotionMove)
            {
                move.Annotation = MoveNotationWriter.AnnotateMove(move, pieces);
                UpdateCellsAndPiecesOfMove(move);
            }
            return move;
        }

        public void UndoMove()
        {
            Move move = Moves.Pop();
            Cell cellOneBefore = move.CellOneBefore;
            if (move.IsPromotionMove)
            {
                Pieces[move.CellOneBefore.Piece!.Color].Add(move.CellOneBefore.Piece);
                Pieces[move.CellOneBefore.Piece.Color].Remove(pieces[move.CellOneBefore.Piece.Color]
                    .First(p => p.Row == move.CellTwoBefore.Row && p.Col == move.CellTwoBefore.Col));
            }
            else
            {
                Pieces[cellOneBefore.Piece!.Color].First(piece => piece.Equals(move.CellTwoAfter.Piece)).SetCoordinates(cellOneBefore);
            }
            Cells[cellOneBefore.Row, cellOneBefore.Col] = cellOneBefore;
            Cells[cellOneBefore.Row, cellOneBefore.Col].Piece = cellOneBefore.Piece;
            var cellTwoBefore = move.CellTwoBefore;
            Cells[cellTwoBefore.Row, cellTwoBefore.Col] = cellTwoBefore;
            Cells[cellTwoBefore.Row, cellTwoBefore.Col].Piece = cellTwoBefore.Piece;

            if (move.CellTwoBefore.Piece != null)
            {
                Pieces[move.CellTwoBefore.Piece.Color].Add(move.CellTwoBefore.Piece);
            }

            if (move.CellThreeBefore != null)
            {
                var cellThreeBefore = move.CellThreeBefore;
                Cells[cellThreeBefore.Row, cellThreeBefore.Col] = cellThreeBefore;
                if (move.CellThreeBefore.Piece != null && move.CellFourBefore == null)
                {
                    Pieces[move.CellThreeBefore.Piece.Color].Add(move.CellThreeBefore.Piece);
                }
                else
                {
                    Pieces[move.CellThreeBefore.Piece!.Color].First(p => p.Equals(move.CellFourAfter!.Piece)).SetCoordinates(cellThreeBefore);
                }
                Cells[cellThreeBefore.Row, cellThreeBefore.Col].Piece = cellThreeBefore.Piece;
            }

            if (move.CellFourBefore != null)
            {
                var cellFourBefore = move.CellFourBefore;
                Cells[cellFourBefore.Row, cellFourBefore.Col] = cellFourBefore;
                Cells[cellFourBefore.Row, cellFourBefore.Col].Piece = cellFourBefore.Piece;
            }

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
                halfMoveCount = 0;
                UpdateFenAnnotation();
            }
        }

        public void UndoPromotionMove()
        {
            Cells[PromotionMove!.CellOneBefore.Row, PromotionMove.CellOneBefore.Col] = PromotionMove.CellOneBefore;
            Cells[PromotionMove.CellOneBefore.Row, PromotionMove.CellOneBefore.Col].Piece = PromotionMove.CellOneBefore.Piece;
            Cells[PromotionMove.CellTwoBefore.Row, PromotionMove.CellTwoBefore.Col] = PromotionMove.CellTwoBefore;
            if (PromotionMove.CellTwoBefore.Piece != null)
            {
                Cells[PromotionMove.CellTwoBefore.Row, PromotionMove.CellTwoBefore.Col].Piece = PromotionMove.CellTwoBefore.Piece;
            }

            Pieces[PromotionMove.CellOneBefore.Piece!.Color].Remove(Pieces[PromotionMove.CellOneBefore.Piece.Color].First(p => p.HasEqualCoordinates(PromotionMove.CellTwoBefore)));
            Pieces[PromotionMove.CellOneBefore.Piece.Color].Add(PromotionMove.CellOneBefore.Piece);

            PromotionMove = null;
        }

        public void PromotePiece(PieceType pieceType)
        {
            var cell = promotionMove.CellTwoAfter;
            cell.Piece = PieceConstructor.ConstructPieceByType(pieceType, TurnColor, cell.Row, cell.Col);
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            PromotionMove!.CellTwoAfter.Piece = cell.Piece;
            PromotionMove.CellTwoAfter.Piece.SetCoordinates(cell);
            PromotionMove.Annotation = MoveNotationWriter.AnnotateMove(promotionMove, pieces);
            Cells[cell.Row, cell.Col].Piece = cell.Piece;
            Cells[cell.Row, cell.Col].Piece!.SetCoordinates(cell);
            Cells[PromotionMove.CellOneBefore.Row, PromotionMove.CellOneBefore.Col].Piece = null;

            Pieces[TurnColor].Remove(Pieces[TurnColor].First(p => p.HasEqualCoordinates(cell)));
            var oppositeColorPieceToRemove = pieces[oppositeColor].FirstOrDefault(p => p.HasEqualCoordinates(cell));
            if (oppositeColorPieceToRemove != null)
            {
                Pieces[oppositeColor].Remove(oppositeColorPieceToRemove);
            }

            Pieces[cell.Piece.Color].Add(cell.Piece);
        }

        public void FinishMove(Move move, Cell selectedCell)
        {
            if (selectedCell != null)
            {
                if (selectedCell.Piece != null && (move.CellTwoBefore.Piece != null || move.CellThreeBefore != null && move.CellFourBefore == null))
                {
                    if (move.CellThreeBefore != null && move.CellFourBefore == null)
                    {
                        Pieces[move.CellThreeBefore.Piece!.Color].Remove(Pieces[move.CellThreeBefore.Piece.Color].First(p => p.Equals(move.CellThreeBefore.Piece)));
                    }
                    else
                    {
                        Pieces[move.CellTwoBefore.Piece!.Color].Remove(Pieces[move.CellTwoBefore.Piece.Color].First(p => p.Equals(move.CellTwoBefore.Piece)));
                    }
                }
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
            if (PromotionMove != null)
            {
                PromotionMove = null;
            }
            move.CurrHalfMoveCount = halfMoveCount;
            this.ReverseTurnColor();
            Moves.Push(move);
            UpdateFenAnnotation();
            Moves.Peek().FenAnnotation = this.fenAnnotation;
        }

        public void CalculatePossibleMoves()
        {
            var king = (King)Pieces[TurnColor].First(p => p.PieceType == PieceType.King);
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            king.Attackers.Clear();
            king.IsInCheck = false;
            king.Defenders = KingDefenderFinder.FindDefenders(king, TurnColor);
            (Cells[king.Row, king.Col].Piece as King)!.IsInCheck = false;

            foreach (var piece in Pieces[oppositeColor])
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece);
                piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                Cells[piece.Row, piece.Col].Piece = piece;
                var checkedKingCell = piece.LegalMoves.FirstOrDefault(c => c.Piece != null && c.Piece.PieceType == PieceType.King);
                if (checkedKingCell != null)
                {
                    king.Attackers.Add(piece);
                    king.IsInCheck = true;
                }
            }
            if (king.IsInCheck && moves.Any())
            {
                moves.Peek().Annotation += "+";
            }
            var validMovesToStopCheck = new List<Cell>();
            if (king.Attackers.Count == 1)
            {
                validMovesToStopCheck = CheckDirectionFinder.GetLegalMovesToStopCheck(king, king.Attackers.First(), this);
            }
            if (king.Attackers.Count > 1)
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(king);
                king.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                foreach (var piece in Pieces[TurnColor].Where(p => p.PieceType != PieceType.King))
                {
                    piece.LegalMoves = new List<Cell>();
                    piece.ProtectedCells = new List<Cell>();
                }
            }
            else
            {
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
                        movesToPreventPotentialCheck.Remove(movesToPreventPotentialCheck.FirstOrDefault(c => c.Row == currDefenderAndPotentialAttacker.Item1.Row && c.Col == currDefenderAndPotentialAttacker.Item1.Col)!);
                        movesToPreventPotentialCheck.Add(Cells[currDefenderAndPotentialAttacker.Item2.Row, currDefenderAndPotentialAttacker.Item2.Col]);
                        if (king.Attackers.Count == 1 && king.Attackers.First().PieceType == PieceType.Knight)
                        {
                            legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = new List<Cell>();
                        }
                        legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Where(lm => movesToPreventPotentialCheck.Contains(lm)).ToList();
                    }
                    piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                    piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                    Cells[piece.Row, piece.Col].Piece = piece;
                }
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
                    if (Moves.Any())
                    {
                        Moves.Peek().Annotation = moves.Peek().Annotation.Replace("+", "#");
                        if (oppositeColor == PieceColor.White)
                        {
                            Moves.Peek().Annotation += " 1-0";
                        }
                        else
                        {
                            Moves.Peek().Annotation += " 0-1";
                        }
                    }
                    return true;
                }
                else
                {
                    GameResult = "Stalemate!";
                    Moves.Peek().Annotation += " 1/2-1/2";
                    return true;
                }
            }
            if (CheckForDraw())
            {
                GameResult = "Draw!";
                Moves.Peek().Annotation += " 1/2-1/2";
                return true;
            }
            else if (Moves.Count > 7)
            {
                if (CheckForThreefoldRepetition())
                {
                    GameResult = "Draw! Threefold repetition!";
                    Moves.Peek().Annotation += " 1/2-1/2";
                    return true;
                }
            }
            return false;
        }

        public void RestoreBackupCells()
        {
            foreach (var cell in BackupCells.Where(c => c.Row != 7 && c.Row != 0 && !c.HasEqualRowAndCol(promotionMove.CellOneBefore)))
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
            Cells[0, 4].Piece = new King(PieceColor.Black, 0, 4);
            Cells[7, 4].Piece = new King(PieceColor.White, 7, 4);
        }

        private void SetupQueens()
        {
            Cells[0, 3].Piece = new Queen(PieceColor.Black, 0, 3);
            Cells[7, 3].Piece = new Queen(PieceColor.White, 7, 3);
        }

        private void SetupBishops()
        {
            Cells[0, 2].Piece = new Bishop(PieceColor.Black, 0, 2);
            Cells[0, 5].Piece = new Bishop(PieceColor.Black, 0, 5);

            Cells[7, 2].Piece = new Bishop(PieceColor.White, 7, 2);
            Cells[7, 5].Piece = new Bishop(PieceColor.White, 7, 5);
        }

        private void SetupKnights()
        {
            Cells[0, 1].Piece = new Knight(PieceColor.Black, 0, 1);
            Cells[0, 6].Piece = new Knight(PieceColor.Black, 0, 6);

            Cells[7, 1].Piece = new Knight(PieceColor.White, 7, 1);
            Cells[7, 6].Piece = new Knight(PieceColor.White, 7, 6);
        }

        private void SetupPawns()
        {
            int row = 1;
            for (int col = 0; col < Cells.GetLength(0); col++)
            {
                Cells[row, col].Piece = new Pawn(PieceColor.Black, row, col);
            }
            row = 6;
            for (int col = 0; col < Cells.GetLength(0); col++)
            {
                Cells[row, col].Piece = new Pawn(PieceColor.White, row, col);
            }
        }

        private void SetupRooks()
        {
            Cells[0, 0].Piece = new Rook(PieceColor.Black, 0, 0);
            Cells[0, 7].Piece = new Rook(PieceColor.Black, 0, 7);

            Cells[7, 0].Piece = new Rook(PieceColor.White, 7, 0);
            Cells[7, 7].Piece = new Rook(PieceColor.White, 7, 7);
        }

        private void SetupPiecesDemo()
        {
            Cells[6, 3].Piece = new Pawn(PieceColor.White, 6, 3);
            Cells[6, 4].Piece = new Pawn(PieceColor.White, 6, 4);

            Cells[1, 3].Piece = new Pawn(PieceColor.Black, 1, 3);
            Cells[1, 4].Piece = new Pawn(PieceColor.Black, 1, 4);

            Cells[6, 6].Piece = new Bishop(PieceColor.White, 6, 6);
            Cells[1, 6].Piece = new Bishop(PieceColor.Black, 1, 6);

            Cells[4, 4].Piece = new Knight(PieceColor.White, 4, 4);
            Cells[3, 3].Piece = new Knight(PieceColor.Black, 3, 3);

            Cells[7, 7].Piece = new Rook(PieceColor.White, 7, 7);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, 0, 0);

            Cells[6, 2].Piece = new Queen(PieceColor.White, 6, 2);
            Cells[3, 4].Piece = new Queen(PieceColor.Black, 3, 4);

            Cells[7, 4].Piece = new King(PieceColor.White, 7, 4);
            Cells[0, 4].Piece = new King(PieceColor.Black, 0, 4);

        }

        private void SetupPiecesCheckBishopTest()
        {
            Cells[6, 3].Piece = new Bishop(PieceColor.White, 6, 3);
            Cells[0, 2].Piece = new Bishop(PieceColor.Black, 0, 2);

            Cells[6, 6].Piece = new King(PieceColor.White, 6, 6);
            Cells[1, 6].Piece = new King(PieceColor.Black, 1, 6);
        }

        private void SetupPiecesCheckRookTest()
        {
            Cells[7, 7].Piece = new Rook(PieceColor.White, 7, 7);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, 0, 0);

            Cells[6, 6].Piece = new King(PieceColor.White, 6, 6);
            Cells[1, 6].Piece = new King(PieceColor.Black, 1, 6);
        }
        private void SetupPiecesEnPassantTest()
        {
            Cells[3, 2].Piece = new Pawn(PieceColor.White, 3, 2);
            Cells[6, 6].Piece = new Pawn(PieceColor.White, 6, 6);

            Cells[1, 1].Piece = new Pawn(PieceColor.Black, 1, 1);
            Cells[4, 5].Piece = new Pawn(PieceColor.Black, 4, 5);

            Cells[7, 4].Piece = new King(PieceColor.White, 7, 4);
            Cells[1, 4].Piece = new King(PieceColor.Black, 1, 4);
        }

        private void SetupPiecesPromotionTest()
        {
            Cells[1, 1].Piece = new Pawn(PieceColor.White, 1, 1);
            Cells[1, 6].Piece = new Pawn(PieceColor.White, 1, 6);

            Cells[6, 1].Piece = new Pawn(PieceColor.Black, 6, 1);
            Cells[6, 7].Piece = new Pawn(PieceColor.Black, 6, 7);

            Cells[0, 5].Piece = new Queen(PieceColor.Black, 0, 5);
            Cells[0, 6].Piece = new Queen(PieceColor.Black, 0, 6);



            Cells[2, 7].Piece = new King(PieceColor.White, 2, 7);
            //Cells[0, 0].Piece = new King(PieceColor.White, 0, 0);
            Cells[1, 5].Piece = new King(PieceColor.Black, 1, 5);
        }

        private void SetupPiecesStalemateTest()
        {
            Cells[6, 7].Piece = new Pawn(PieceColor.Black, 6, 7);

            //Cells[2, 6].Piece = new Queen(PieceColor.White, 2, 6);
            Cells[7, 7].Piece = new Bishop(PieceColor.White, 7, 7);
            Cells[0, 0].Piece = new Bishop(PieceColor.Black, 0, 0);
            //Cells[6, 2].Piece = new Knight(PieceColor.Black, 6, 2);

            Cells[6, 6].Piece = new King(PieceColor.White, 6, 6);
            Cells[1, 4].Piece = new King(PieceColor.Black, 1, 4);
        }

        private void SetupPiecesKnightCheckTest()
        {
            Cells[6, 3].Piece = new Knight(PieceColor.White, 6, 3);
            Cells[7, 5].Piece = new Queen(PieceColor.White, 7, 5);

            Cells[3, 5].Piece = new Pawn(PieceColor.Black, 3, 5);

            Cells[7, 4].Piece = new King(PieceColor.White, 7, 4);

            Cells[2, 5].Piece = new King(PieceColor.Black, 2, 5);
        }

        private void SetupPiecesKnookTest()
        {
            Cells[6, 3].Piece = new Pawn(PieceColor.White, 6, 3);
            Cells[6, 4].Piece = new Pawn(PieceColor.White, 6, 4);

            Cells[1, 3].Piece = new Pawn(PieceColor.Black, 1, 3);
            Cells[1, 4].Piece = new Pawn(PieceColor.Black, 1, 4);

            Cells[6, 6].Piece = new Bishop(PieceColor.White, 6, 6);
            Cells[1, 6].Piece = new Bishop(PieceColor.Black, 1, 6);

            Cells[4, 4].Piece = new Knight(PieceColor.White, 4, 4);
            Cells[3, 3].Piece = new Knight(PieceColor.Black, 3, 3);

            Cells[7, 7].Piece = new Rook(PieceColor.White, 7, 7);
            Cells[0, 0].Piece = new Rook(PieceColor.Black, 0, 0);

            Cells[6, 2].Piece = new Knook(PieceColor.White, 6, 2);
            Cells[3, 4].Piece = new Knook(PieceColor.Black, 3, 4);

            Cells[7, 4].Piece = new King(PieceColor.White, 7, 4);
            Cells[0, 4].Piece = new King(PieceColor.Black, 0, 4);
        }

        private void SetupPiecesAnnotationTest()
        {
            Cells[4, 5].Piece = new Pawn(PieceColor.Black, 4, 5);


            Cells[3, 4].Piece = new Bishop(PieceColor.White, 3, 4);
            Cells[3, 6].Piece = new Bishop(PieceColor.White, 3, 6);
            Cells[5, 4].Piece = new Bishop(PieceColor.White, 5, 4);
            Cells[5, 6].Piece = new Bishop(PieceColor.White, 5, 6);

            Cells[2, 4].Piece = new Knight(PieceColor.White, 2, 4);
            Cells[2, 6].Piece = new Knight(PieceColor.White, 2, 6);
            Cells[6, 4].Piece = new Knight(PieceColor.White, 6, 4);
            Cells[6, 6].Piece = new Knight(PieceColor.White, 6, 6);

            Cells[7, 0].Piece = new King(PieceColor.White, 7, 0);
            Cells[0, 0].Piece = new King(PieceColor.Black, 0, 0);
        }

        private void SetupPiecesEnPassantCheckSaveTest()
        {
            //Black En passant 
            Cells[5, 7].Piece = new Pawn(PieceColor.White, 5, 7);
            Cells[6, 6].Piece = new Pawn(PieceColor.White, 6, 6);

            Cells[4, 7].Piece = new Pawn(PieceColor.Black, 4, 7);
            Cells[3, 6].Piece = new Pawn(PieceColor.Black, 3, 6);

            Cells[2, 2].Piece = new Rook(PieceColor.White, 2, 2);

            Cells[0, 0].Piece = new King(PieceColor.White, 0, 0);
            Cells[3, 7].Piece = new King(PieceColor.Black, 3, 7);


            //White En passant
            //Cells[2, 0].Piece = new Pawn(PieceColor.Black, 2, 0);
            //Cells[1, 1].Piece = new Pawn(PieceColor.Black, 1, 1);

            //Cells[3, 0].Piece = new Pawn(PieceColor.White, 3, 0);
            //Cells[4, 1].Piece = new Pawn(PieceColor.White, 4, 1);

            //Cells[5, 7].Piece = new Rook(PieceColor.Black, 5, 7);

            //Cells[3, 7].Piece = new King(PieceColor.Black, 3, 7);
            //Cells[4, 0].Piece = new King(PieceColor.White, 4, 0);
            //TurnColor = PieceColor.Black;
        }

        private void SetupPiecesBeforeMoveCheckTest()
        {
            Cells[0, 7].Piece = new Rook(PieceColor.White, 0, 7);
            TurnColor = PieceColor.Black;

            //Cells[0, 7].Piece = new Rook(PieceColor.Black, 0, 7);
            //TurnColor = PieceColor.White;

            Cells[7, 7].Piece = new King(PieceColor.White, 7, 7);
            Cells[0, 0].Piece = new King(PieceColor.Black, 0, 0);
        }

        private void SetupPiecesPawnCheckTest()
        {

            Cells[6, 3].Piece = new Pawn(PieceColor.White, 6, 3);
            Cells[3, 6].Piece = new Pawn(PieceColor.Black, 3, 6);

            Cells[5, 5].Piece = new King(PieceColor.White, 5, 5);
            Cells[3, 4].Piece = new King(PieceColor.Black, 3, 4);
        }

        private void AddPiecesFromSetup()
        {
            var flattenedCells = Cells.Cast<Cell>().ToArray();
            foreach (var cell in flattenedCells.Where(c => c.Piece != null))
            {
                pieces[cell.Piece!.Color].Add(cell.Piece);
            }
        }

        private Move CreateMove(Cell movedToCell, Cell selectedCell)
        {
            Move move = new Move();
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            move.CellOneBefore = new Cell(selectedCell.Row, selectedCell.Col);

            move.CellOneBefore.Piece = PieceConstructor.ConstructPieceByType(selectedCell.Piece!.PieceType, TurnColor, move.CellOneBefore.Row, move.CellOneBefore.Col);
            move.CellTwoBefore = new Cell(movedToCell.Row, movedToCell.Col, movedToCell.Piece);
            if (movedToCell.Piece != null)
            {
                move.CellTwoBefore.Piece = PieceConstructor.ConstructPieceByType(movedToCell.Piece.PieceType, oppositeColor, move.CellTwoBefore.Row, move.CellTwoBefore.Col);
            }

            move.CellOneAfter = new Cell(selectedCell.Row, selectedCell.Col, null);

            move.CellTwoAfter = new Cell(movedToCell.Row, movedToCell.Col);
            move.CellTwoAfter.Piece = PieceConstructor.ConstructPieceByType(selectedCell.Piece.PieceType, TurnColor, move.CellTwoAfter.Row, move.CellTwoAfter.Col);
            if (move.CellTwoAfter.Piece!.PieceType == PieceType.King
                && Math.Abs(move.CellOneBefore.Col - move.CellTwoAfter.Col) == 2)
            {
                move = MoveRookInCastlingMove(move);
                Cells[move.CellThreeBefore!.Row, move.CellThreeBefore.Col].Piece = move.CellThreeAfter!.Piece;
            }
            else if (selectedCell.Piece.PieceType == PieceType.Pawn)
            {
                if (move.CellTwoAfter.Piece.PieceType == PieceType.Pawn
                    && (move.CellTwoAfter.Row == 0 || move.CellTwoAfter.Row == 7))
                {
                    var pawn = move.CellTwoAfter.Piece;
                    move.IsPromotionMove = true;
                    UpdateCellsAndPiecesOfMove(move);
                    CreatePromotionMove(move, pawn);
                }
                if (Moves.Count > 0)
                {
                    var lastMove = Moves.Peek();
                    if (Math.Abs(lastMove.CellOneBefore.Row - lastMove.CellTwoBefore.Row) == 2)
                    {
                        var lastMovedPiece = lastMove.CellTwoAfter.Piece;
                        if (lastMovedPiece!.PieceType == PieceType.Pawn
                            && movedToCell.Col == move.CellTwoAfter.Piece.Col
                            && move.CellTwoBefore.Piece == null
                            && Math.Abs(lastMovedPiece.Col - move.CellOneBefore.Col) == 1
                            && lastMovedPiece.Col == move.CellTwoBefore.Col
                            && lastMovedPiece.Row == move.CellOneBefore.Row)
                        {
                            move = EnPassantMove(move);
                        }
                    }
                }
            }
            return move;
        }

        private Move MoveRookInCastlingMove(Move move)
        {
            var king = move.CellTwoAfter.Piece;
            var colDiff = move.CellOneBefore.Col - move.CellTwoBefore.Col;
            var cell = new Cell(0, 0);
            var selectedCell = new Cell(-1, -1);
            var rookMove = new Move();
            switch (king!.Color)
            {
                case PieceColor.White:
                    if (colDiff < 0)
                    {
                        cell = Cells[king.Row, king.Col - 1];
                        selectedCell = Cells[king.Row, king.Col + 1];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    if (colDiff > 0)
                    {
                        cell = Cells[king.Row, king.Col + 1];
                        selectedCell = Cells[king.Row, king.Col - 2];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    break;
                case PieceColor.Black:
                    if (colDiff < 0)
                    {
                        cell = Cells[king.Row, king.Col - 1];
                        selectedCell = Cells[king.Row, king.Col + 1];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    if (colDiff > 0)
                    {
                        cell = Cells[king.Row, king.Col + 1];
                        selectedCell = Cells[king.Row, king.Col - 2];
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

        private void CreatePromotionMove(Move move, Piece pawn)
        {
            BackupCells = new List<Cell>();
            var promotionCellsPieces = new List<PieceType>() { PieceType.Queen, PieceType.Knight, PieceType.Rook, PieceType.Bishop, PieceType.Knook };

            var rowIncrement = 0;
            switch (pawn.Color)
            {
                case PieceColor.White:
                    for (int i = 0; i < 5; i++)
                    {
                        var currCell = new Cell(pawn.Row + rowIncrement++, pawn.Col);
                        if (currCell.HasEqualRowAndCol(move.CellTwoBefore))
                        {
                            BackupCells.Add(currCell);
                            if (move.CellTwoBefore.Piece != null)
                            {
                                currCell.Piece = PieceConstructor.ConstructPieceByType(move.CellTwoBefore.Piece.PieceType, move.CellTwoBefore.Piece.Color, currCell.Row, currCell.Col);
                                BackupCells[i].Piece = currCell.Piece;
                            }
                        }
                        else
                        {
                            BackupCells.Add(currCell);

                            if (Cells[currCell.Row, currCell.Col].Piece != null)
                            {
                                BackupCells[i].Piece = PieceConstructor.ConstructPieceByType(Cells[currCell.Row, currCell.Col].Piece!.PieceType, Cells[currCell.Row, currCell.Col].Piece!.Color, Cells[currCell.Row, currCell.Col].Row, Cells[currCell.Row, currCell.Col].Col);
                            }
                        }
                        Cells[currCell.Row, currCell.Col].Piece = PieceConstructor.ConstructPieceForPromotion(promotionCellsPieces[i], pawn.Color);
                    }
                    break;
                case PieceColor.Black:
                    for (int i = 0; i < 5; i++)
                    {
                        var currCell = new Cell(pawn.Row - rowIncrement++, pawn.Col);
                        if (currCell.HasEqualRowAndCol(move.CellTwoBefore))
                        {
                            BackupCells.Add(currCell);
                            if (move.CellTwoBefore.Piece != null)
                            {
                                currCell.Piece = PieceConstructor.ConstructPieceByType(move.CellTwoBefore.Piece.PieceType, move.CellTwoBefore.Piece.Color, currCell.Row, currCell.Col);
                                BackupCells[i].Piece = currCell.Piece;
                            }
                        }
                        else
                        {
                            BackupCells.Add(currCell);

                            if (Cells[currCell.Row, currCell.Col].Piece != null)
                            {
                                BackupCells[i].Piece = PieceConstructor.ConstructPieceByType(Cells[currCell.Row, currCell.Col].Piece!.PieceType, Cells[currCell.Row, currCell.Col].Piece!.Color, Cells[currCell.Row, currCell.Col].Row, Cells[currCell.Row, currCell.Col].Col);
                            }
                        }
                        Cells[currCell.Row, currCell.Col].Piece = PieceConstructor.ConstructPieceForPromotion(promotionCellsPieces[i], pawn.Color);
                    }
                    break;
            }
            PromotionMove = move;
        }

        private Move EnPassantMove(Move move)
        {
            var pawn = move.CellTwoAfter.Piece;
            switch (pawn!.Color)
            {
                case PieceColor.White:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Cells[move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col].Piece!.PieceType, PieceColor.Black, move.CellThreeBefore.Row, move.CellThreeBefore.Col);
                    break;
                case PieceColor.Black:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Cells[move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col].Piece!.PieceType, PieceColor.White, move.CellThreeBefore.Row, move.CellThreeBefore.Col);
                    break;
            }
            move.CellThreeAfter = new Cell(move.CellThreeBefore!.Row, move.CellThreeBefore.Col, null);
            return move;
        }

        private void UpdateCellsAndPiecesOfMove(Move move)
        {
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            Cells[move.CellOneBefore.Row, move.CellOneBefore.Col].Piece = null;
            

            if (move.CellTwoBefore.Piece != null)
            {
                Pieces[oppositeColor].Remove(Pieces[oppositeColor].FirstOrDefault(p => p.Equals(move.CellTwoBefore.Piece))!);
            }
            Cells[move.CellTwoBefore.Row, move.CellTwoBefore.Col].Piece = move.CellTwoAfter.Piece;
            Pieces[TurnColor].FirstOrDefault(p => p.HasEqualCoordinates(move.CellOneBefore))!.SetCoordinates(move.CellTwoBefore);


            if (move.CellThreeBefore != null)
            {
                if (move.CellThreeBefore.Piece != null)
                {
                    if (move.CellThreeBefore.Piece.Color != TurnColor)
                    {
                        Pieces[move.CellThreeBefore.Piece.Color].Remove(Pieces[move.CellThreeBefore.Piece.Color].FirstOrDefault(p => p.Equals(move.CellThreeBefore.Piece))!);
                    }
                }

                Cells[move.CellThreeBefore.Row, move.CellThreeBefore.Col].Piece = move.CellThreeAfter!.Piece;
            }

            if (move.CellFourBefore != null)
            {
                Pieces[TurnColor].FirstOrDefault(p => p.Equals(move.CellThreeBefore!.Piece))!.SetCoordinates(move.CellFourAfter!);
                Cells[move.CellFourBefore!.Row, move.CellFourBefore.Col].Piece = move.CellFourAfter!.Piece;
            }
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
                    if ((Pieces[PieceColor.White].First(p => p.PieceType == PieceType.Bishop).HasEvenCoordinates()
                        && Pieces[PieceColor.Black].First(p => p.PieceType == PieceType.Bishop).HasEvenCoordinates())
                        || !Pieces[PieceColor.White].First(p => p.PieceType == PieceType.Bishop).HasEvenCoordinates()
                        && !Pieces[PieceColor.Black].First(p => p.PieceType == PieceType.Bishop).HasEvenCoordinates())
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
                && (movesAsArray[movesAsArray.Length - 2].Equals(movesAsArray[movesAsArray.Length - 6])
                && movesAsArray[movesAsArray.Length - 2].IsOppositeMove(movesAsArray[movesAsArray.Length - 4]))
                && (movesAsArray[movesAsArray.Length - 4].Equals(movesAsArray[movesAsArray.Length - 8])
                && movesAsArray[movesAsArray.Length - 4].IsOppositeMove(movesAsArray[movesAsArray.Length - 6])))
                )
            {
                movesAreRepeated = true;
            }
            return movesAreRepeated;
        }

    }
}
