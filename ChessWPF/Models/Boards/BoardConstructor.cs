using ChessWPF.Contracts.Pieces;
using ChessWPF.Game;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Models.Data.Board
{
    public sealed class BoardConstructor
    {
        private PieceColor turnColor;
        private Dictionary<PieceColor, HashSet<ConstructorMenuPiece>> menuPieces;
        private ConstructorCell[,] constructorCells;

        public BoardConstructor()
        {
            CreateConstructorCells();
            CreateConstructorPieces();
        }

        public PieceColor TurnColor
        {
            get { return turnColor; }
            set { turnColor = value; }
        }

        public Dictionary<PieceColor, HashSet<ConstructorMenuPiece>> ConstructorPieces
        {
            get { return menuPieces; }
            private set { menuPieces = value; }
        }
        public ConstructorCell[,] ConstructorCells
        {
            get { return constructorCells; }
            private set { constructorCells = value; }
        }

        public void CreateConstructorCells()
        {
            ConstructorCells = new ConstructorCell[8, 8];
            for (int row = 0; row < ConstructorCells.GetLength(0); row++)
            {
                for (int col = 0; col < ConstructorCells.GetLength(1); col++)
                {
                    ConstructorCells[row, col] = new ConstructorCell(row, col);
                }
            }
        }

        public void ImportPosition(Position position)
        {
            foreach (var piece in position.Pieces.Keys.SelectMany(color => position.Pieces[color]))
            {
                ConstructorCells[piece.Row, piece.Col].UpdatePiece(new ConstructorBoardPiece(piece.Row, piece.Col, piece.Color, piece.PieceType));
            }
        }

        public Position ExportPosition()
        {
            var position = new Position();
            position.Pieces = FindPieces();

            return position;
        }

        public void UpdateCellPiece(int row, int col, IConstructorPiece? constructorPiece)
        {
            ConstructorCells[row, col].UpdatePiece(constructorPiece);
        }

        private void CreateConstructorPieces()
        {
            ConstructorPieces = new Dictionary<PieceColor, HashSet<ConstructorMenuPiece>>
            {
                { PieceColor.White, CreatePieceCollection(PieceColor.White) },
                { PieceColor.Black, CreatePieceCollection(PieceColor.Black) }
            };
        }

        private HashSet<ConstructorMenuPiece> CreatePieceCollection(PieceColor color)
        {
            var pieces = new HashSet<ConstructorMenuPiece>();
            var pieceTypes = Enum.GetValues<PieceType>().ToList();
            foreach (var pieceType in pieceTypes)
            {
                pieces.Add(new ConstructorMenuPiece(pieceType, color));
            }
            return pieces;
        }

        private Dictionary<PieceColor, List<Piece>> FindPieces()
        {
            var pieces = new Dictionary<PieceColor, List<Piece>>()
            {
                [PieceColor.White] = new List<Piece>(),
                [PieceColor.Black] = new List<Piece>()
            };
            var constructorCellsFlattened = ConstructorCells.Cast<ConstructorCell>().ToArray();
            foreach (var cell in constructorCellsFlattened.Where(c => c.ConstructorBoardPiece != null))
            {
                pieces[cell.ConstructorBoardPiece.Color].Add(PieceConstructor.ConstructPieceByType(
                    cell.ConstructorBoardPiece.PieceType,
                    cell.ConstructorBoardPiece.Color,
                    cell.Row,
                    cell.Col));
            }

            return pieces;
        }
    }
}
