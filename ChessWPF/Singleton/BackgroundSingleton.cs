using ChessWPF.Game;
using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Singleton
{
    public sealed class BackgroundSingleton
    {
        private static BackgroundSingleton instance = null;
        private static readonly object padlock = new object();
        private Board board;
        private List<Cell> legalMoves = new List<Cell>();
        private bool cellsAreUpdated;
        private BoardViewModel boardViewModel;
        private CellViewModel selectedCell;





        public BackgroundSingleton()
        {

        }

        public static BackgroundSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BackgroundSingleton();
                    }
                    return instance;
                }
            }
        }
        public BoardViewModel BoardViewModel
        {
            get { return boardViewModel; }
            set { boardViewModel = value; }
        }
        public Board Board { get => boardViewModel.Board; set => boardViewModel.Board = value; }
        public List<Cell> LegalMoves
        {
            get { return legalMoves; }
        }
        public bool CellsAreUpdated { get => cellsAreUpdated; set => cellsAreUpdated = value; }
        public CellViewModel SelectedCell { get => selectedCell; set => selectedCell = value; }


        public void StartGame()
        {
            BoardViewModel.StartGame();
        }

        public void ResetBoard()
        {
            UnselectSelectedCell();
            BoardViewModel.ResetBoard();
        }

        public void SelectCell(CellViewModel cellViewModel)
        {
            SelectedCell = BoardViewModel.CellViewModels[cellViewModel.Cell.Row][cellViewModel.Cell.Col];
            GetLegalMoves(SelectedCell.Cell.Piece);
        }

        public void MovePiece(Cell cell)
        {
            BoardViewModel.MovePiece(cell, SelectedCell.Cell);
            ClearLegalMoves();
            SelectedCell = null;
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.GameHasEnded = true;
            }
        }

        public void UndoMove()
        {
            if (SelectedCell != null)
            {
                UnselectSelectedCell();
            }
            BoardViewModel.UndoMove();
        }

        public void SelectPieceForPromotion(CellViewModel cellViewModel)
        {
            BoardViewModel.PromotePiece(cellViewModel.Cell.Piece.PieceType);
            if (BoardViewModel.GameResult != null)
            {
                BoardViewModel.GameHasEnded = true;
            }
        }

        private void GetLegalMoves(Piece piece)
        {
            //var newLegalMoves = LegalMoveFinder.GetLegalMoves(piece);
            var newLegalMoves = SelectedCell.Cell.Piece.LegalMoves;
            if (newLegalMoves != legalMoves)
            {
                ClearLegalMoves();
                ShowNewLegalMoves(newLegalMoves);
            }
        }

        private void ShowNewLegalMoves(List<Cell> newLegalMoves)
        {
            legalMoves = newLegalMoves;

            foreach (var cell in newLegalMoves)
            {
                boardViewModel.CellViewModels[cell.Row][cell.Col].CanBeMovedTo = true;
            }
        }

        private void ClearLegalMoves()
        {
            foreach (var cell in legalMoves)
            {
                boardViewModel.CellViewModels[cell.Row][cell.Col].CanBeMovedTo = false;
            }
            legalMoves = new List<Cell>();
        }
        private void UnselectSelectedCell()
        {
            SelectedCell = null;
            if (LegalMoves.Any())
            {
                ClearLegalMoves();
            }
        }

        private void UpdateBoard()
        {
            Board = BoardViewModel.Board;
        }
    }
}
