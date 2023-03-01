using ChessWPF.Commands;
using ChessWPF.Constants;
using ChessWPF.Game;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        private Board board;
        private CellViewModel[][] cellViewModels;
        private bool cellsAreUpdated;
        private bool gameHasStarted;
        private PieceColor turnColor;
        private List<Cell> backupCells;
        private Move promotionMove;
        private bool gameHasEnded;

        private string gameResult;

        public string GameResult
        {
            get { return gameResult; }
            set
            {
                gameResult = value;
                OnPropertyChanged(nameof(GameResult));
            }
        }
        public bool GameHasEnded
        {
            get { return gameHasEnded; }
            set
            {
                gameHasEnded = value;
                OnPropertyChanged(nameof(GameHasEnded));
            }
        }

        public BoardViewModel()
        {
            Board = new Board();
            cellViewModels = new CellViewModel[8][];
            MatchCellViewModelsToCells();
            TurnColor = PieceColor.White;
            backupCells = new List<Cell>();
            EndGameCommand = new EndGameCommand(this);
        }

        public Board Board
        {
            get
            {
                return board;
            }
            set
            {
                board = value;
                OnPropertyChanged(nameof(Board));
            }
        }

        public CellViewModel[][] CellViewModels
        {
            get
            {
                return cellViewModels;
            }
            set
            {
                cellViewModels = value;
                OnPropertyChanged(nameof(CellViewModels));
            }
        }
        public bool CellsAreUpdated
        {
            get { return cellsAreUpdated; }
            set
            {
                cellsAreUpdated = value;
                OnPropertyChanged(nameof(CellsAreUpdated));
            }
        }
        public bool GameHasStarted
        {
            get { return gameHasStarted; }
            set
            {
                gameHasStarted = value;
                OnPropertyChanged(nameof(GameHasStarted));
            }
        }


        public PieceColor TurnColor
        {
            get { return turnColor; }
            set { turnColor = value; }
        }

        public ICommand EndGameCommand { get; set; }

        public void MatchCellViewModelsToCells()
        {
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                cellViewModels[row] = new CellViewModel[8];
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    cellViewModels[row][col] = new CellViewModel(board.Cells[row, col]);
                }
            }
        }
        public void StartGame()
        {
            TurnColor = PieceColor.White;
            MarkWhichPiecesCanBeSelected();
        }

        public void ResetBoard()
        {
            Board = new Board();
            GameHasStarted = false;
            GameHasEnded = false;
            GameResult = null;
            //cellViewModels = new CellViewModel[8][];
            //MatchCellViewModelsToCells();
            ResetMatchCellViewModelsToCells();
            StartGame();
        }



        public void PrepareForNextTurn()
        {
            if (gameHasEnded)
            {
                MakeAllPiecesUnselectable();
                return;
            }
            ReverseTurnColor();
            //FindChecks();
            MarkWhichPiecesCanBeSelected();
        }


        public void MovePiece(Cell cell, CellViewModel selectedCell)
        {
            var selectedPieceType = selectedCell.Cell.Piece.PieceType;
            Move move = CreateMove(cell, selectedCell);
            if (move.CellTwoAfter.Piece.PieceType == PieceType.King && Math.Abs(move.CellOneBefore.Col - move.CellTwoAfter.Col) == 2)
            {
                move = MoveRookInCastlingMove(move);
            }
            else if (selectedPieceType == PieceType.Pawn)
            {
                if (move.CellTwoAfter.Piece.PieceType == PieceType.Pawn && (move.CellTwoAfter.Row == 0 || move.CellTwoAfter.Row == 7))
                {
                    var pawn = move.CellTwoAfter.Piece;
                    CreatePromotionMove(move, pawn);
                    MarkWhichPiecesCanBeSelected();
                }
                if (Board.Moves.Count > 0)
                {
                    var lastMove = board.Moves.Peek();
                    if (Math.Abs(lastMove.CellOneBefore.Row - lastMove.CellTwoBefore.Row) == 2)
                    {
                        var lastMovedPiece = lastMove.CellTwoAfter.Piece;
                        if (lastMovedPiece.PieceType == PieceType.Pawn
                            && cell.Col == move.CellTwoAfter.Piece.Cell.Col
                            && move.CellTwoBefore.Piece == null
                            && Math.Abs(lastMovedPiece.Cell.Col - move.CellOneBefore.Col) == 1
                            && lastMovedPiece.Cell.Row == move.CellOneAfter.Row)
                        {
                            move = EnPassantMove(move);
                        }
                    }
                }
            }
            if (backupCells.Count == 0)
            {
                FinishMove(move, selectedCell);
            }
        }

        public void UndoMove()
        {
            Move move = Board.Moves.Pop();
            Cell cellOneBefore = move.CellOneBefore;
            CellViewModels[cellOneBefore.Row][cellOneBefore.Col].Cell = cellOneBefore;
            CellViewModels[cellOneBefore.Row][cellOneBefore.Col].Cell.Piece = cellOneBefore.Piece;
            Board.Cells[cellOneBefore.Row, cellOneBefore.Col] = cellOneBefore;
            CellViewModels[cellOneBefore.Row][cellOneBefore.Col].UpdateCellImage();

            Cell cellTwoBefore = move.CellTwoBefore;
            Cell cellTwoAfter = move.CellTwoAfter;
            CellViewModels[cellTwoBefore.Row][cellTwoBefore.Col].Cell = cellTwoBefore;
            CellViewModels[cellTwoBefore.Row][cellTwoBefore.Col].Cell.Piece = cellTwoBefore.Piece;
            Board.Cells[cellTwoBefore.Row, cellTwoBefore.Col] = cellTwoBefore;

            CellViewModels[cellTwoBefore.Row][cellTwoBefore.Col].UpdateCellImage();

            if (move.CellThreeBefore != null)
            {
                Cell cellThreeBefore = move.CellThreeBefore;
                CellViewModels[cellThreeBefore.Row][cellThreeBefore.Col].Cell = cellThreeBefore;
                CellViewModels[cellThreeBefore.Row][cellThreeBefore.Col].Cell.Piece = cellThreeBefore.Piece;
                Board.Cells[cellThreeBefore.Row, cellThreeBefore.Col] = cellThreeBefore;
                CellViewModels[cellThreeBefore.Row][cellThreeBefore.Col].UpdateCellImage();
            }
            if (move.CellFourBefore != null)
            {
                Cell cellFourBefore = move.CellFourBefore;
                CellViewModels[cellFourBefore.Row][cellFourBefore.Col].Cell = cellFourBefore;
                CellViewModels[cellFourBefore.Row][cellFourBefore.Col].Cell.Piece = cellFourBefore.Piece;
                Board.Cells[cellFourBefore.Row, cellFourBefore.Col] = cellFourBefore;
                CellViewModels[cellFourBefore.Row][cellFourBefore.Col].UpdateCellImage();
            }
            if (!Board.Moves.Any())
            {
                GameHasStarted = false;
            }
            if (GameHasEnded)
            {
                GameHasEnded = false;
                GameResult = null;
            }
            PrepareForNextTurn();
        }

        public void PromotePiece(PieceType pieceType)
        {
            var cell = promotionMove.CellTwoAfter;
            cell.Piece = PieceConstructor.ConstructPieceByType(pieceType, TurnColor, cell);
            promotionMove.CellTwoAfter.Piece = cell.Piece;
            promotionMove.CellTwoAfter.Piece.Cell = cell;
            CellViewModels[cell.Row][cell.Col].Cell.Piece = cell.Piece;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.Cell = cell;
            CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = false;
            RestoreBackupCells();
            FinishMove(promotionMove, null);
        }

        private void ResetMatchCellViewModelsToCells()
        {
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    cellViewModels[row][col].Cell = board.Cells[row, col];
                    cellViewModels[row][col].UpdateCellImage();
                    cellViewModels[row][col].CanBeMovedTo = false;
                    cellViewModels[row][col].IsSelected = false;
                }
            }
        }

        private void ReverseTurnColor()
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

        private void MarkWhichPiecesCanBeSelected()
        {
            Board.Pieces[PieceColor.White] = new List<Piece>();
            Board.Pieces[PieceColor.Black] = new List<Piece>();
            if (backupCells.Count > 0)
            {
                MakeAllPiecesUnselectable();
            }
            else
            {
                var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
                for (int row = 0; row < CellViewModels.Length; row++)
                {
                    for (int col = 0; col < CellViewModels.Length; col++)
                    {
                        CellViewModels[row][col].Cell = Board.Cells[row, col];
                        var currCellViewModel = CellViewModels[row][col];
                        if (currCellViewModel.IsInCheck)
                        {
                            currCellViewModel.IsInCheck = false;
                        }
                        if (Board.Cells[row, col].Piece == null)
                        {
                            currCellViewModel.CanBeSelected = false;
                        }
                        else if (Board.Cells[row, col].Piece.Color == TurnColor)
                        {
                            Board.Pieces[TurnColor].Add(Board.Cells[row, col].Piece);
                            currCellViewModel.CanBeSelected = true;
                        }
                        else
                        {
                            Board.Pieces[oppositeColor].Add(Board.Cells[row, col].Piece);

                            currCellViewModel.CanBeSelected = false;
                        }
                        currCellViewModel.UpdateCellImage();
                    }
                }

                var king = (King)Board.Pieces[TurnColor].First(p => p.PieceType == PieceType.King);
                king.Attackers.Clear();
                king.Defenders = KingDefenderFinder.FindDefenders(king, TurnColor);
                CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = false;

                foreach (var piece in Board.Pieces[oppositeColor])
                {
                    var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece);
                    piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                    piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                    var checkedKingCell = piece.LegalMoves.FirstOrDefault(c => c.Piece != null && c.Piece.PieceType == PieceType.King);
                    if (checkedKingCell != null)
                    {
                        king.Attackers.Add(piece);
                        king.IsInCheck = true;
                        CellViewModels[king.Cell.Row][king.Cell.Col].IsInCheck = true;
                    }
                }
                var validMovesToStopCheck = new List<Cell>();
                if (king.Attackers.Count == 1)
                {
                    validMovesToStopCheck = CheckDirectionFinder.GetLegalMovesToStopCheck(king, king.Attackers.First(), board);
                }
                foreach (var piece in Board.Pieces[TurnColor])
                {
                    var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece);
                    if (validMovesToStopCheck.Count > 0 && piece.PieceType != PieceType.King && !king.Defenders.Any(d => d.Item1 == piece))
                    {
                        legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves]
                            .Where(lm => validMovesToStopCheck.Contains(lm)).ToList();
                    }
                    else if (king.Defenders.Any(d => d.Item1 == piece))
                    {
                        var currDefenderAndPotentialAttacker = king.Defenders.First(d => d.Item1 == piece);
                        var movesToPreventPotentialCheck = CheckDirectionFinder.GetLegalMovesToStopCheck(king, currDefenderAndPotentialAttacker.Item2, Board);
                        movesToPreventPotentialCheck.Remove(currDefenderAndPotentialAttacker.Item1.Cell);
                        movesToPreventPotentialCheck.Add(currDefenderAndPotentialAttacker.Item2.Cell);
                        legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Where(lm => movesToPreventPotentialCheck.Contains(lm)).ToList();
                    }
                    piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                    piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                }
                if (Board.Pieces[TurnColor].Sum(p => p.LegalMoves.Count) == 0)
                {
                    if (king.Attackers.Count > 0)
                    {
                        GameResult = $"{oppositeColor} wins by checkmate!";
                    }
                    else
                    {
                        GameResult = "Stalemate!";
                    }
                }
                if (CheckForDraw())
                {
                    GameResult = "Draw!";
                    MakeAllPiecesUnselectable();
                }
                else if (Board.Moves.Count > 9)
                {
                    if (CheckForThreefoldRepetition())
                    {
                        GameResult = "Draw by threefold repetition!";
                        MakeAllPiecesUnselectable();

                    }

                }
            }
        }

        private bool CheckForThreefoldRepetition()
        {
            var movesAreRepeated = false;

            var movesAsArray = Board.Moves.Reverse().ToArray();
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
                movesAreRepeated = true;
            }
            //else if (movesAsArray[movesAsArray.Length - 2].Equals(movesAsArray[movesAsArray.Length - 4])
            //    && movesAsArray[movesAsArray.Length - 2].Equals(movesAsArray[movesAsArray.Length - 6]))
            //{
            //    movesAreRepeated = true;
            //}


            return movesAreRepeated;
        }

        private void MakeAllPiecesUnselectable()
        {
            foreach (var cellViewModelRow in CellViewModels)
            {
                foreach (var cellViewModel in cellViewModelRow)
                {
                    cellViewModel.CanBeSelected = false;
                }
            }
        }

        private bool CheckForDraw()
        {
            var isGameDrawn = false;
            if (Board.Pieces.Sum(p => p.Value.Count) == 2)
            {
                isGameDrawn = true;
            }
            else if (Board.Pieces.Sum(p => p.Value.Count) == 3)
            {
                if (Board.Pieces.Any(p => p.Value.Count == 2 && p.Value.Any(p => p.PieceType == PieceType.Bishop || p.PieceType == PieceType.Knight)))
                {
                    isGameDrawn = true;
                }
            }
            else if (Board.Pieces.Sum(p => p.Value.Count) == 4)
            {
                if (Board.Pieces.All(p => p.Value.Count == 2 && p.Value.Any(p => p.PieceType == PieceType.Bishop)))
                {
                    if ((Board.Pieces[PieceColor.White].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell()
                        && Board.Pieces[PieceColor.Black].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell())
                        || !Board.Pieces[PieceColor.White].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell()
                        && !Board.Pieces[PieceColor.Black].First(p => p.PieceType == PieceType.Bishop).Cell.IsEvenCell())
                    {
                        return true;
                    }
                }
            }

            return isGameDrawn;
        }

        private Move CreateMove(Cell cell, CellViewModel selectedCell)
        {
            Move move = new Move();
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            move.CellOneBefore = new Cell(selectedCell.Cell.Row, selectedCell.Cell.Col);
            move.CellOneBefore.Piece = PieceConstructor.ConstructPieceByType(selectedCell.Cell.Piece.PieceType, TurnColor, move.CellOneBefore);
            move.CellTwoBefore = new Cell(cell.Row, cell.Col, cell.Piece);
            if (cell.Piece != null)
            {
                move.CellTwoBefore.Piece = PieceConstructor.ConstructPieceByType(cell.Piece.PieceType, oppositeColor, move.CellTwoBefore);
            }

            CellViewModels[cell.Row][cell.Col].Cell.Piece = selectedCell.Cell.Piece;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.PieceType = selectedCell.Cell.Piece.PieceType;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.Color = selectedCell.Cell.Piece.Color;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.Cell = new Cell(cell.Row, cell.Col, CellViewModels[cell.Row][cell.Col].Cell.Piece);

            move.CellOneAfter = new Cell(selectedCell.Cell.Row, selectedCell.Cell.Col, null);

            move.CellTwoAfter = new Cell(cell.Row, cell.Col);
            move.CellTwoAfter.Piece = PieceConstructor.ConstructPieceByType(cell.Piece.PieceType, TurnColor, move.CellTwoAfter);
            selectedCell.Cell.Piece = null;

            return move;
        }

        private Move EnPassantMove(Move move)
        {
            var pawn = move.CellTwoAfter.Piece;
            switch (pawn.Color)
            {
                case PieceColor.White:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Board.Cells[move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col].Piece.PieceType, PieceColor.Black, move.CellThreeBefore);
                    break;
                case PieceColor.Black:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Board.Cells[move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col].Piece.PieceType, PieceColor.White, move.CellThreeBefore);
                    break;
            }
            Board.Cells[move.CellThreeBefore.Row, move.CellThreeBefore.Col].Piece = null;
            move.CellThreeAfter = new Cell(move.CellThreeBefore.Row, move.CellThreeBefore.Col, null);
            return move;
        }

        private Move MoveRookInCastlingMove(Move move)
        {
            var king = move.CellTwoAfter.Piece;
            var colDiff = move.CellOneBefore.Col - move.CellTwoBefore.Col;
            var cell = new Cell(0, 0);
            var selectedCell = new CellViewModel(cell);
            var rookMove = new Move();
            switch (king.Color)
            {
                case PieceColor.White:
                    if (colDiff < 0)
                    {
                        cell = Board.Cells[king.Cell.Row, king.Cell.Col - 1];
                        selectedCell = CellViewModels[king.Cell.Row][king.Cell.Col + 1];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    if (colDiff > 0)
                    {
                        cell = Board.Cells[king.Cell.Row, king.Cell.Col + 1];
                        selectedCell = CellViewModels[king.Cell.Row][king.Cell.Col - 2];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    break;
                case PieceColor.Black:
                    if (colDiff < 0)
                    {
                        cell = Board.Cells[king.Cell.Row, king.Cell.Col - 1];
                        selectedCell = CellViewModels[king.Cell.Row][king.Cell.Col + 1];
                        rookMove = CreateMove(cell, selectedCell);
                    }
                    if (colDiff > 0)
                    {
                        cell = Board.Cells[king.Cell.Row, king.Cell.Col + 1];
                        selectedCell = CellViewModels[king.Cell.Row][king.Cell.Col - 2];
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
            backupCells = new List<Cell>();
            var rowIncrement = 0;
            var promotionCellsPieces = new List<PieceType>() { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };
            switch (pawn.Color)
            {
                case PieceColor.White:
                    for (int i = 0; i < 4; i++)
                    {
                        var currCell = new Cell(pawn.Cell.Row + rowIncrement++, pawn.Cell.Col);
                        backupCells.Add(currCell);
                        if (Board.Cells[currCell.Row, currCell.Col].Piece != null)
                        {
                            backupCells[i].Piece = PieceConstructor.ConstructPieceByType(Board.Cells[currCell.Row, currCell.Col].Piece.PieceType, Board.Cells[currCell.Row, currCell.Col].Piece.Color, Board.Cells[currCell.Row, currCell.Col]);
                        }
                        Board.Cells[currCell.Row, currCell.Col].Piece = PieceConstructor.ConstructPieceForPromotion(promotionCellsPieces[i], pawn.Color);
                        CellViewModels[currCell.Row][currCell.Col].CanBeSelectedForPromotion = true;
                        CellViewModels[currCell.Row][currCell.Col].UpdateCellImage();
                    }
                    break;
                case PieceColor.Black:
                    for (int i = 0; i < 4; i++)
                    {
                        var currCell = new Cell(pawn.Cell.Row - rowIncrement++, pawn.Cell.Col);
                        backupCells.Add(currCell);
                        if (Board.Cells[currCell.Row, currCell.Col].Piece != null)
                        {
                            backupCells[i].Piece = PieceConstructor.ConstructPieceByType(Board.Cells[currCell.Row, currCell.Col].Piece.PieceType, Board.Cells[currCell.Row, currCell.Col].Piece.Color, Board.Cells[currCell.Row, currCell.Col]);
                        }
                        Board.Cells[currCell.Row, currCell.Col].Piece = PieceConstructor.ConstructPieceForPromotion(promotionCellsPieces[i], pawn.Color);
                        CellViewModels[currCell.Row][currCell.Col].CanBeSelectedForPromotion = true;
                        CellViewModels[currCell.Row][currCell.Col].UpdateCellImage();
                    }
                    break;
            }
            promotionMove = move;
        }

        private void FinishMove(Move move, CellViewModel selectedCell)
        {
            if (selectedCell != null)
            {
                selectedCell.Cell.Piece = null;
            }

            CellViewModels[move.CellOneAfter.Row][move.CellOneAfter.Col].UpdateCellImage();

            CellViewModels[move.CellTwoAfter.Row][move.CellTwoAfter.Col].UpdateCellImage();
            if (move.CellThreeBefore != null)
            {
                CellViewModels[move.CellThreeAfter.Row][move.CellThreeAfter.Col].UpdateCellImage();
            }
            if (move.CellFourBefore != null)
            {
                CellViewModels[move.CellFourAfter.Row][move.CellFourAfter.Col].UpdateCellImage();
            }
            Board.Moves.Push(move);
            if (!GameHasStarted)
            {
                GameHasStarted = true;
            }
            PrepareForNextTurn();
        }

        private void RestoreBackupCells()
        {
            foreach (var cell in backupCells.Where(c => c.Row != 7 && c.Row != 0))
            {
                Board.Cells[cell.Row, cell.Col] = cell;
                if (cell.Piece != null)
                {
                    Board.Cells[cell.Row, cell.Col].Piece = cell.Piece;
                }
                else
                {
                    Board.Cells[cell.Row, cell.Col].Piece = null;
                }
                CellViewModels[cell.Row][cell.Col].CanBeSelectedForPromotion = false;
            }
            backupCells = new List<Cell>();
        }
    }
}
