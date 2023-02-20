using ChessWPF.Commands;
using ChessWPF.Constants;
using ChessWPF.Game;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
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







        public BoardViewModel()
        {
            Board = new Board();
            cellViewModels = new CellViewModel[8][];
            MatchCellViewModelsToCells();
            TurnColor = PieceColor.White;
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

        public void MatchCellViewModelsToCells()
        {
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                cellViewModels[row] = new CellViewModel[8];
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    cellViewModels[row][col] = new CellViewModel(board.Cells[row, col]);
                    //cellViewModels[row][col].UpdateCellImage();
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
            //cellViewModels = new CellViewModel[8][];
            //MatchCellViewModelsToCells();
            ResetMatchCellViewModelsToCells();
            StartGame();
        }



        public void PrepareForNextTurn()
        {
            ReverseTurnColor();
            //FindChecks();
            MarkWhichPiecesCanBeSelected();
        }


        public void MovePiece(Cell cell, CellViewModel selectedCell)
        {
            Move move = CreateMove(cell, selectedCell);
            if (move.CellTwoAfter.Piece.PieceType == PieceType.King && Math.Abs(move.CellOneBefore.Col - move.CellTwoAfter.Col) == 2)
            {
                move = MoveRookInCastlingMove(move);
            }
            if (Board.Moves.Count > 0 && selectedCell.Cell.Piece.PieceType == PieceType.Pawn)
            {
                var lastMovedPiece = board.Moves.Peek().CellTwoAfter.Piece;
                if (lastMovedPiece.PieceType == PieceType.Pawn && cell.Col == move.CellTwoAfter.Piece.Cell.Col && lastMovedPiece.Cell.Col == move.CellTwoAfter.Piece.Cell.Col &&  move.CellTwoBefore.Piece == null)
                {
                    move = EnPassantMove(move);
                }
            }
            //CellViewModels[selectedCell.Cell.Row][selectedCell.Cell.Col].IsSelected = false;
            selectedCell.Cell.Piece = null;
            CellViewModels[move.CellOneBefore.Row][move.CellOneBefore.Col].UpdateCellImage();
            CellViewModels[move.CellOneAfter.Row][move.CellOneAfter.Col].UpdateCellImage();
            CellViewModels[move.CellTwoBefore.Row][move.CellTwoBefore.Col].UpdateCellImage();
            CellViewModels[move.CellTwoAfter.Row][move.CellTwoAfter.Col].UpdateCellImage();
            if (move.CellThreeBefore != null)
            {
                CellViewModels[move.CellThreeBefore.Row][move.CellThreeBefore.Col].UpdateCellImage();
                CellViewModels[move.CellThreeAfter.Row][move.CellThreeAfter.Col].UpdateCellImage();
            }
            if (move.CellFourBefore != null)
            {
                CellViewModels[move.CellFourBefore.Row][move.CellFourBefore.Col].UpdateCellImage();
                CellViewModels[move.CellFourAfter.Row][move.CellFourAfter.Col].UpdateCellImage();
            }
            Board.Moves.Push(move);
            if (!GameHasStarted)
            {
                GameHasStarted = true;
            }
            PrepareForNextTurn();
        }

        private Move CreateMove(Cell cell, CellViewModel selectedCell)
        {
            Move move = new Move();
            move.CellOneBefore = new Cell(selectedCell.Cell.Row, selectedCell.Cell.Col);
            move.CellOneBefore.Piece = PieceConstructor.ConstructPieceByType(selectedCell.Cell.Piece, move.CellOneBefore);
            move.CellTwoBefore = new Cell(cell.Row, cell.Col, cell.Piece);
            if (cell.Piece != null)
            {
                move.CellTwoBefore.Piece = PieceConstructor.ConstructPieceByType(cell.Piece, move.CellTwoBefore);
            }

            CellViewModels[cell.Row][cell.Col].Cell.Piece = selectedCell.Cell.Piece;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.PieceType = selectedCell.Cell.Piece.PieceType;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.Color = selectedCell.Cell.Piece.Color;
            CellViewModels[cell.Row][cell.Col].Cell.Piece.Cell = new Cell(cell.Row, cell.Col, CellViewModels[cell.Row][cell.Col].Cell.Piece);

            move.CellOneAfter = new Cell(selectedCell.Cell.Row, selectedCell.Cell.Col, null);

            move.CellTwoAfter = new Cell(cell.Row, cell.Col);
            move.CellTwoAfter.Piece = PieceConstructor.ConstructPieceByType(cell.Piece, move.CellTwoAfter);
            //selectedCell.Cell.Piece = null;

            return move;
        }

        private Move EnPassantMove(Move move)
        {
            var pawn = move.CellTwoAfter.Piece;
            switch (pawn.Color)
            {
                case PieceColor.White:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Board.Cells[move.CellTwoBefore.Row + 1, move.CellTwoBefore.Col].Piece, move.CellThreeBefore);
                    break;
                case PieceColor.Black:
                    move.CellThreeBefore = new Cell(move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col);
                    move.CellThreeBefore.Piece = PieceConstructor.ConstructPieceByType(Board.Cells[move.CellTwoBefore.Row - 1, move.CellTwoBefore.Col].Piece, move.CellThreeBefore);

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
            PrepareForNextTurn();
        }

        private void ResetMatchCellViewModelsToCells()
        {
            for (int row = 0; row < board.Cells.GetLength(0); row++)
            {
                //cellViewModels[row] = new CellViewModel[8];
                for (int col = 0; col < board.Cells.GetLength(1); col++)
                {
                    cellViewModels[row][col].Cell = board.Cells[row, col];
                    //cellViewModels[row][col].Cell.Piece = board.Cells[row, col].Piece;
                    //cellViewModels[row][col].Cell.Piece.Cell = cellViewModels[row][col].Cell;
                    cellViewModels[row][col].UpdateCellImage();
                    cellViewModels[row][col].CanBeMovedTo = false;
                    cellViewModels[row][col].IsSelected = false;
                    //cellViewModels[row][col].UpdateCanBeSelected();
                    //cellViewModels[row][col].UpdateCellImage();
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
            var oppositeColor = TurnColor == PieceColor.White ? PieceColor.Black : PieceColor.White;
            foreach (var cellViewModelsRows in cellViewModels)
            {
                foreach (var cellViewModel in cellViewModelsRows/*.Where(cr => cr.Cell.Piece != null)*/)
                {
                    if (cellViewModel.Cell.Piece == null)
                    {
                        cellViewModel.CanBeSelected = false;
                    }
                    else if (cellViewModel.Cell.Piece.Color == TurnColor)
                    {
                        Board.Pieces[TurnColor].Add(cellViewModel.Cell.Piece);
                        cellViewModel.CanBeSelected = true;
                    }
                    else
                    {
                        Board.Pieces[oppositeColor].Add(cellViewModel.Cell.Piece);
                        cellViewModel.CanBeSelected = false;
                    }
                    //cellViewModel.Cell.Piece.ValidMoves = LegalMoveFinder.GetLegalMoves(cellViewModel.Cell.Piece);
                }
                //foreach (var cellViewModel in cellViewModelsRows.Where(cr => cr.Cell.Piece != null && cr.Cell.Piece.Color != TurnColor))
                //{
                //    cellViewModel.CanBeSelected = false;
                //    cellViewModel.Cell.Piece.ValidMoves = LegalMoveFinder.GetLegalMoves(cellViewModel.Cell.Piece);
                //    Board.Pieces[oppositeColor].Add(cellViewModel.Cell.Piece);
                //}
            }

            var king = (King)Board.Pieces[TurnColor].First(p => p.PieceType == PieceType.King);
            king.Attackers.Clear();
            king.Defenders = KingDefenderFinder.FindDefenders(king, TurnColor);
            foreach (var piece in Board.Pieces[oppositeColor])
            {
                var legalMovesAndProtectedCells = LegalMoveFinder.GetLegalMovesAndProtectedCells(piece);
                piece.LegalMoves = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves];
                piece.ProtectedCells = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.ProtectedCells];
                var checkedKingCell = piece.LegalMoves.FirstOrDefault(c => c.Piece != null && c.Piece.PieceType == PieceType.King);
                if (checkedKingCell != null)
                {
                    //King king = (King)checkedKingCell.Piece;
                    king.Attackers.Add(piece);
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
                    legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves] = legalMovesAndProtectedCells[LegalMovesAndProtectedCells.LegalMoves].Where(lm => validMovesToStopCheck.Contains(lm)).ToList();
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
        }
    }
}
