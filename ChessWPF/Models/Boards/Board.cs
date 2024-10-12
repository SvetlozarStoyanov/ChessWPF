using ChessWPF.Constants;
using ChessWPF.Game;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Moves;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using ChessWPF.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Models.Boards
{
    public sealed class Board
    {
        private string gameResult;
        private string fenAnnotation;
        private bool gameHasStarted;
        private bool gameHasEnded;
        private int halfMoveCount;
        private PieceColor turnColor;
        private Position startingPosition;
        private Move? ongoingPromotionMove;
        private Stack<Move> moves;
        private Move movesTree;
        private Cell[,] cells;
        private List<Cell> backupCells;
        private Dictionary<PieceColor, List<Piece>> pieces;

        public Board()
        {
            CreateCells();
            Moves = new Stack<Move>();
            Pieces = new Dictionary<PieceColor, List<Piece>>
            {
                { PieceColor.White, new List<Piece>() },
                { PieceColor.Black, new List<Piece>() }
            };
            BackupCells = new List<Cell>();
        }

        public string GameResult
        {
            get => gameResult;
            set => gameResult = value;
        }

        public string FenAnnotation
        {
            get => fenAnnotation;
            private set => fenAnnotation = value;
        }

        public bool GameHasStarted
        {
            get => gameHasStarted;
            set => gameHasStarted = value;
        }

        public bool GameHasEnded
        {
            get => gameHasEnded;
            set => gameHasEnded = value;
        }

        public int HalfMoveCount
        {
            get => halfMoveCount;
            private set => halfMoveCount = value;
        }

        public Position StartingPosition
        {
            get { return startingPosition; }
            private set { startingPosition = value; }
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

        public Move MovesTree 
        { 
            get => movesTree;
            set 
            {
                movesTree = value;
            }
        }

        public void CreateCells()
        {
            Cells = new Cell[8, 8];
            for (int row = 0; row < Cells.GetLength(0); row++)
            {
                for (int col = 0; col < Cells.GetLength(1); col++)
                {
                    Cells[row, col] = new Cell(row, col);
                }
            }
        }

        public void ImportPosition(string fenAnnotation)
        {
            var position = FenAnnotationReader.GetPosition(fenAnnotation);
            if (StartingPosition == null)
            {
                StartingPosition = position;
            }
            foreach (var color in position.Pieces.Keys)
            {
                foreach (var piece in position.Pieces[color])
                {
                    var currPiece = PieceCreator.CreatePieceByProperties(piece.PieceType,
                        piece.Color,
                        piece.Row,
                        piece.Col);
                    Cells[piece.Row, piece.Col].UpdateCell(currPiece);
                    Pieces[piece.Color].Add(currPiece);
                }
            }
            TurnColor = position.TurnColor;
            HalfMoveCount = position.HalfMoveCount;
            FenAnnotation = position.FenAnnotation;
        }

        public void Reset()
        {
            foreach (var piece in Pieces[PieceColor.White])
            {
                Cells[piece.Row, piece.Col].UpdateCell(null);
            }
            Pieces[PieceColor.White].Clear();
            foreach (var piece in Pieces[PieceColor.Black])
            {
                Cells[piece.Row, piece.Col].UpdateCell(null);
            }
            Pieces[PieceColor.Black].Clear();
            if (Moves.Any())
            {
                UpdateCellsMovedTo(Moves.Peek());
                TurnColor = Moves.Reverse().FirstOrDefault()!.CellOneBefore.Piece!.Color;
                Moves.Clear();
            }
            HalfMoveCount = 0;
            ImportPosition(StartingPosition.FenAnnotation);
        }

        public void ReverseTurnColor()
        {
            if (TurnColor == PieceColor.White)
            {
                TurnColor = PieceColor.Black;
            }
            else
            {
                TurnColor = PieceColor.White;
            }
        }

        public void UpdateFenAnnotation()
        {
            FenAnnotation = FenAnnotationWriter.WriteFenAnnotationFromBoard(this);
        }

        public Move MovePiece(Cell movedToCell, Cell selectedCell)
        {
            var move = CreateMove(movedToCell, selectedCell);

            if (!move.IsPromotionMove)
            {
                move.Annotation = MoveNotationWriter.AnnotateMove(move, Pieces);
                UpdateCellsAndPiecesOfMove(move);
            }
            return move;
        }

        public void UndoMove()
        {
            var king = (Pieces[TurnColor].FirstOrDefault(p => p.PieceType == PieceType.King) as King);
            if (king!.IsInCheck)
            {
                UnCheckKing(king!);
            }
            var move = Moves.Pop();
            MovesTree = MovesTree.ParentMove;
            var cellOneBefore = move.CellOneBefore;
            var cellTwoBefore = move.CellTwoBefore;
            if (move.IsPromotionMove)
            {
                Pieces[cellOneBefore.Piece!.Color].Add(cellOneBefore.Piece!);
                Pieces[cellOneBefore.Piece!.Color].Remove(pieces[cellOneBefore.Piece!.Color]
                    .First(p => p.HasEqualCoordinatesWithCell(cellTwoBefore)));
            }
            else
            {
                Pieces[cellOneBefore.Piece!.Color].First(piece => piece.Equals(move.CellTwoAfter.Piece)).SetCoordinates(cellOneBefore);
            }
            Cells[cellOneBefore.Row, cellOneBefore.Col].UpdateCell(cellOneBefore.Piece);

            Cells[cellTwoBefore.Row, cellTwoBefore.Col].UpdateCell(move.CellTwoBefore.Piece);

            if (cellTwoBefore.Piece != null)
            {
                Pieces[cellTwoBefore.Piece.Color].Add(cellTwoBefore.Piece!);
            }

            if (move.CellThreeBefore != null)
            {
                var cellThreeBefore = move.CellThreeBefore;
                Cells[cellThreeBefore.Row, cellThreeBefore.Col].UpdateCell(cellThreeBefore.Piece);
                if (move.CellThreeBefore.Piece != null && move.CellFourBefore == null)
                {
                    Pieces[move.CellThreeBefore.Piece.Color].Add(move.CellThreeBefore.Piece);
                }
                else
                {
                    var cellFourBefore = move.CellFourBefore;
                    Cells[cellFourBefore!.Row, cellFourBefore.Col].UpdateCell(cellFourBefore.Piece);
                    Pieces[cellThreeBefore.Piece!.Color].FirstOrDefault(p => p.Equals(move.CellFourAfter!.Piece))!.SetCoordinates(cellThreeBefore);
                }
            }
            UpdateCellsMovedTo(move);
            ReverseTurnColor();
            if (moves.Any())
            {
                var lastMove = moves.Peek();
                FenAnnotation = lastMove.FenAnnotation;
                if (lastMove.IsHalfMove())
                {
                    HalfMoveCount = lastMove.CurrHalfMoveCount;
                }
                UpdateCellsMovedTo(lastMove);
            }
            else
            {
                HalfMoveCount = StartingPosition.HalfMoveCount;
                FenAnnotation = StartingPosition.FenAnnotation;
                //UpdateFenAnnotation();
            }
        }

        public void UndoOngoingPromotionMove()
        {
            Cells[OngoingPromotionMove!.CellOneBefore.Row, OngoingPromotionMove.CellOneBefore.Col].UpdateCell(OngoingPromotionMove.CellOneBefore.Piece);

            Cells[OngoingPromotionMove.CellTwoBefore.Row, OngoingPromotionMove.CellTwoBefore.Col].UpdateCell(OngoingPromotionMove.CellTwoBefore.Piece);
            if (OngoingPromotionMove.CellTwoBefore.Piece != null)
            {
                Pieces[OngoingPromotionMove.CellTwoBefore.Piece!.Color].Add(OngoingPromotionMove.CellTwoBefore.Piece!);
            }

            Pieces[OngoingPromotionMove.CellOneBefore.Piece!.Color].Remove(Pieces[OngoingPromotionMove.CellOneBefore.Piece.Color].First(p => p.HasEqualCoordinatesWithCell(OngoingPromotionMove.CellTwoBefore)));
            Pieces[OngoingPromotionMove.CellOneBefore.Piece.Color].Add(OngoingPromotionMove.CellOneBefore.Piece);

            OngoingPromotionMove = null;
            CalculatePossibleMoves();
        }

        public void PromotePiece(PieceType pieceType)
        {
            var cell = OngoingPromotionMove!.CellTwoAfter;
            cell.Piece = PieceCreator.CreatePieceByProperties(pieceType, TurnColor, cell.Row, cell.Col);
            OngoingPromotionMove.Annotation = MoveNotationWriter.AnnotateMove(OngoingPromotionMove, Pieces);
            UpdateCellsAndPiecesOfMove(OngoingPromotionMove);
        }

        public void FinishMove(Move move, Cell selectedCell)
        {
            if (move.IsHalfMove())
            {
                HalfMoveCount++;
            }
            else
            {
                HalfMoveCount = 0;
            }
            if (OngoingPromotionMove != null)
            {
                OngoingPromotionMove = null;
            }
            if (move.CellOneBefore.Piece!.PieceType == PieceType.King)
            {
                Cells[move.CellOneBefore.Row, move.CellOneBefore.Col].MarkAsUnchecked();
            }
            if (Moves.Any())
            {
                UpdateCellsMovedTo(Moves.Peek());
            }
            move.CurrHalfMoveCount = halfMoveCount;
            ReverseTurnColor();
            Moves.Push(move);
            UpdateFenAnnotation();
            Moves.Peek().FenAnnotation = FenAnnotation;
            UpdateCellsMovedTo(move);
            if (MovesTree == null)
            {
                MovesTree = move;
            }
            else
            {
                move.ParentMove = MovesTree;
                MovesTree.ChildMoves.Add(move);
                MovesTree = move;
            }
        }

        public void CalculatePossibleMoves()
        {
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var oppositeKing = (King)Pieces[oppositeColor].First(p => p.PieceType == PieceType.King);
            UnCheckKing(oppositeKing);
            var king = (King)Pieces[TurnColor].First(p => p.PieceType == PieceType.King);
            king.Defenders = KingDefenderFinder.FindDefenders(king, TurnColor, this);
            king.Attackers.Clear();
            foreach (var piece in Pieces[oppositeColor])
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece, FenAnnotation, TurnColor, cells, Pieces);
                piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                Cells[piece.Row, piece.Col].Piece = piece;
                var checkedKingCell = piece.LegalMoves.FirstOrDefault(c => c.Piece != null && c.Piece.PieceType == PieceType.King);
                if (checkedKingCell != null)
                {
                    king.Attackers.Add(piece);
                }
            }
            if (king.Attackers.Any())
            {
                Cells[king.Row, king.Col].MarkAsChecked();
                king.IsInCheck = true;
                if (Moves.Any())
                {
                    Moves.Peek().Annotation += "+";
                }
            }
            var validMovesToStopCheck = new List<Cell>();
            if (king.Attackers.Count == 1)
            {
                validMovesToStopCheck = LegalMovesToStopCheckFinder.GetLegalMovesToStopCheck(king, king.Attackers.First(), this);
            }
            if (king.Attackers.Count > 1)
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(king, FenAnnotation, TurnColor, cells, Pieces);
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
                    var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece, FenAnnotation, TurnColor, cells, Pieces);
                    if (validMovesToStopCheck.Count > 0 && piece.PieceType != PieceType.King && !king.Defenders.Any(d => d.Item1 == piece))
                    {
                        legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]
                            .Where(lm => validMovesToStopCheck.Contains(lm)).ToList();
                    }
                    else if (king.Defenders.Any(d => d.Item1.HasEqualCoordinates(piece.Row, piece.Col)))
                    {
                        if (king.IsInCheck)
                        {
                            legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = new List<Cell>();
                        }
                        var currDefenderAndPotentialAttacker = king.Defenders.First(d => d.Item1.HasEqualCoordinates(piece.Row, piece.Col));
                        var movesToPreventPotentialCheck = LegalMovesToStopCheckFinder.GetLegalMovesToStopCheck(king, currDefenderAndPotentialAttacker.Item2, this);
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
                    Cells[piece.Row, piece.Col].Piece!.UpdateLegalMovesAndProtectedCells(piece.LegalMoves, piece.ProtectedCells);
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
            foreach (var cell in BackupCells.Where(c => c.Row != 7 && c.Row != 0 /*&& !c.HasEqualRowAndCol(OngoingPromotionMove.CellOneBefore)*/))
            {
                Cells[cell.Row, cell.Col].UpdateCell(cell.Piece);
            }
            BackupCells.Clear();
        }

        public void RestoreAllBackupCells()
        {
            foreach (var cell in BackupCells)
            {
                Cells[cell.Row, cell.Col].UpdateCell(cell.Piece);
            }
            BackupCells.Clear();
        }

        private Move CreateMove(Cell movedToCell, Cell selectedCell)
        {
            Move move = new Move();
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            move.CellOneBefore = new Cell(selectedCell.Row, selectedCell.Col);

            move.CellOneBefore.Piece = PieceCreator.CreatePieceByProperties(selectedCell.Piece!.PieceType, TurnColor, move.CellOneBefore.Row, move.CellOneBefore.Col);
            move.CellTwoBefore = new Cell(movedToCell.Row, movedToCell.Col, movedToCell.Piece);
            if (movedToCell.Piece != null)
            {
                move.CellTwoBefore.Piece = PieceCreator.CreatePieceByProperties(movedToCell.Piece.PieceType, oppositeColor, move.CellTwoBefore.Row, move.CellTwoBefore.Col);
            }

            move.CellOneAfter = new Cell(selectedCell.Row, selectedCell.Col, null);

            move.CellTwoAfter = new Cell(movedToCell.Row, movedToCell.Col);
            move.CellTwoAfter.Piece = PieceCreator.CreatePieceByProperties(selectedCell.Piece.PieceType, TurnColor, move.CellTwoAfter.Row, move.CellTwoAfter.Col);
            if (move.CellTwoAfter.Piece!.PieceType == PieceType.King
                && Math.Abs(move.CellOneBefore.Col - move.CellTwoAfter.Col) == 2)
            {
                move = MoveRookInCastlingMove(move);
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
                var enPassantCoordinatesAnnotation = FenAnnotation.Split(' ', StringSplitOptions.RemoveEmptyEntries)[3];
                if (enPassantCoordinatesAnnotation != "-" && move.CellTwoBefore.HasEqualRowAndCol(GetCellByAnnotation(enPassantCoordinatesAnnotation)))
                {
                    move = EnPassantMove(move);
                }

            }

            return move;
        }

        private Cell GetCellByAnnotation(string annotation)
        {
            return Cells[8 - (int)(annotation[1] - 48), (int)(annotation[0] - 97)];
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
                    for (int i = 0; i < promotionCellsPieces.Count; i++)
                    {
                        var currCell = new Cell(pawn.Row + rowIncrement++, pawn.Col);
                        if (currCell.HasEqualRowAndCol(move.CellTwoBefore))
                        {
                            BackupCells.Add(currCell);
                            if (move.CellTwoBefore.Piece != null)
                            {
                                currCell.Piece = PieceCreator.CreatePieceByProperties(move.CellTwoBefore.Piece.PieceType, move.CellTwoBefore.Piece.Color, currCell.Row, currCell.Col);
                                BackupCells[i].Piece = currCell.Piece;
                            }
                        }
                        else
                        {
                            BackupCells.Add(currCell);
                            if (Cells[currCell.Row, currCell.Col].Piece != null)
                            {
                                BackupCells[i].Piece = PieceCreator.CreatePieceByProperties(Cells[currCell.Row, currCell.Col].Piece!.PieceType, Cells[currCell.Row, currCell.Col].Piece!.Color, currCell.Row, currCell.Col);
                            }
                        }
                        Cells[currCell.Row, currCell.Col].UpdateCellForPromotion(PieceCreator.CreatePieceForPromotion(promotionCellsPieces[i], pawn.Color));
                    }
                    break;
                case PieceColor.Black:
                    for (int i = 0; i < promotionCellsPieces.Count; i++)
                    {
                        var currCell = new Cell(pawn.Row - rowIncrement++, pawn.Col);
                        if (currCell.HasEqualRowAndCol(move.CellTwoBefore))
                        {
                            BackupCells.Add(currCell);
                            if (move.CellTwoBefore.Piece != null)
                            {
                                currCell.Piece = PieceCreator.CreatePieceByProperties(move.CellTwoBefore.Piece.PieceType, move.CellTwoBefore.Piece.Color, currCell.Row, currCell.Col);
                                BackupCells[i].Piece = currCell.Piece;
                            }
                        }
                        else
                        {
                            BackupCells.Add(currCell);

                            if (Cells[currCell.Row, currCell.Col].Piece != null)
                            {
                                BackupCells[i].Piece = PieceCreator.CreatePieceByProperties(Cells[currCell.Row, currCell.Col].Piece!.PieceType, Cells[currCell.Row, currCell.Col].Piece!.Color, currCell.Row, currCell.Col);
                            }
                        }
                        Cells[currCell.Row, currCell.Col].UpdateCellForPromotion(PieceCreator.CreatePieceForPromotion(promotionCellsPieces[i], pawn.Color));
                    }
                    break;
            }
            OngoingPromotionMove = move;
        }

        private Move EnPassantMove(Move move)
        {
            var pawn = move.CellTwoAfter.Piece;
            switch (pawn!.Color)
            {
                case PieceColor.White:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceCreator.CreatePieceByProperties(Cells[move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col].Piece!.PieceType, PieceColor.Black, move.CellThreeBefore.Row, move.CellThreeBefore.Col);
                    break;
                case PieceColor.Black:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceCreator.CreatePieceByProperties(Cells[move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col].Piece!.PieceType, PieceColor.White, move.CellThreeBefore.Row, move.CellThreeBefore.Col);
                    break;
            }
            move.CellThreeAfter = new Cell(move.CellThreeBefore!.Row, move.CellThreeBefore.Col, null);
            return move;
        }

        private void UpdateCellsAndPiecesOfMove(Move move)
        {
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;

            Cells[move.CellOneBefore.Row, move.CellOneBefore.Col].UpdateCell(null);


            if (move.CellTwoBefore.Piece != null)
            {
                Pieces[oppositeColor].Remove(Pieces[oppositeColor].FirstOrDefault(p => p.Equals(move.CellTwoBefore.Piece))!);
            }
            Cells[move.CellTwoBefore.Row, move.CellTwoBefore.Col].UpdateCell(move.CellTwoAfter.Piece);

            var cellTwoAfterPiece = Pieces[TurnColor].FirstOrDefault(p => p.HasEqualCoordinatesWithCell(move.CellOneBefore))!;
            if (cellTwoAfterPiece == null)
            {
                Pieces[TurnColor].Remove(Pieces[TurnColor].FirstOrDefault(p => p.HasEqualCoordinatesWithCell(move.CellTwoAfter))!);
                Pieces[TurnColor].Add(move.CellTwoAfter.Piece!);
            }
            else
            {
                Pieces[TurnColor].FirstOrDefault(p => p.HasEqualCoordinatesWithCell(move.CellOneBefore))!.SetCoordinates(move.CellTwoBefore);
            }


            if (move.CellThreeBefore != null)
            {
                if (move.CellThreeBefore.Piece != null)
                {
                    if (move.CellThreeBefore.Piece.Color != TurnColor)
                    {
                        Pieces[move.CellThreeBefore.Piece.Color].Remove(Pieces[move.CellThreeBefore.Piece.Color].FirstOrDefault(p => p.Equals(move.CellThreeBefore.Piece))!);
                    }
                }
                Cells[move.CellThreeBefore.Row, move.CellThreeBefore.Col].UpdateCell(move.CellThreeAfter!.Piece);
            }

            if (move.CellFourBefore != null)
            {
                Pieces[TurnColor].FirstOrDefault(p => p.Equals(move.CellThreeBefore!.Piece))!.SetCoordinates(move.CellFourAfter!);
                Cells[move.CellFourBefore!.Row, move.CellFourBefore.Col].UpdateCell(move.CellFourAfter!.Piece);
            }
        }

        private void UpdateCellsMovedTo(Move move)
        {
            Cells[move.CellOneBefore.Row, move.CellOneBefore.Col].UpdateMarkAsMovedTo();
            Cells[move.CellTwoBefore.Row, move.CellTwoBefore.Col].UpdateMarkAsMovedTo();
        }

        private void UnCheckKing(King king)
        {
            king.Attackers.Clear();
            king.IsInCheck = false;
            Cells[king.Row, king.Col].MarkAsUnchecked();
        }

        private bool CheckForDraw()
        {
            
            if (HalfMoveCount >= 100)
            {
                GameResult = "Draw! 100 moves were made with no pawn advances or piece captures!";
                return true;
            }
            else if (Pieces.Sum(p => p.Value.Count) == 2)
            {
                GameResult = "Draw!";
                return true;
            }
            else if (Pieces.Sum(p => p.Value.Count) == 3)
            {
                if (Pieces.Any(p => p.Value.Count == 2 && p.Value.Any(p => p.PieceType == PieceType.Bishop || p.PieceType == PieceType.Knight)))
                {
                    GameResult = "Draw! Insufficient pieces to checkmate!";
                    return true;
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
            else if (pieces.All(pc => pc.Value.Count(p => p.PieceType == PieceType.Bishop) == pc.Value.Count - 1))
            {
                if (pieces.All(pc => pc.Value.Where(p => p.PieceType == PieceType.Bishop).All(b => b.HasEvenCoordinates()))
                    || pieces.All(pc => pc.Value.Where(p => p.PieceType == PieceType.Bishop).All(b => !b.HasEvenCoordinates())))
                {
                    GameResult = "Draw! Insufficient pieces to checkmate!";
                    return true;
                }
            }

            return false;
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
