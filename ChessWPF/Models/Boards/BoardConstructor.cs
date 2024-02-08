using ChessWPF.Contracts.Pieces;
using ChessWPF.Game;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using ChessWPF.Models.Positions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessWPF.Models.Boards
{
    public sealed class BoardConstructor
    {
        private PieceColor turnColor;
        private ValueTuple<int, int>? enPassantCoordinates;
        private ValueTuple<bool, bool, bool, bool> castlingRights;
        private HashSet<CellCoordinates?> enPassantPosibilities;
        private bool[] castlingPosibilities;
        private Dictionary<PieceColor, HashSet<ConstructorMenuPiece>> menuPieces;
        private char[,] simplifiedCells;
        private ConstructorCell[,] constructorCells;

        public BoardConstructor()
        {
            CreateCells();
            CreateConstructorPieces();
            EnPassantPossibilities = new HashSet<CellCoordinates?>();
        }

        public event UpdateCastlingPosibilitiesEventHandler CastlingPosibilitiesUpdate;
        public delegate void UpdateCastlingPosibilitiesEventHandler(object? sender, EventArgs e);
        public event UpdateEnPassantPosibilitiesEventHandler EnPassantPosibilitiesUpdate;
        public delegate void UpdateEnPassantPosibilitiesEventHandler(object? sender, EventArgs e);

        public PieceColor TurnColor
        {
            get => turnColor;
            set => turnColor = value;
        }

        public ValueTuple<int, int>? EnPassantCoordinates
        {
            get => enPassantCoordinates;
            private set => enPassantCoordinates = value;
        }

        public ValueTuple<bool, bool, bool, bool> CastlingRights
        {
            get => castlingRights;
            private set => castlingRights = value;
        }

        public HashSet<CellCoordinates?> EnPassantPossibilities
        {
            get { return enPassantPosibilities; }
            private set { enPassantPosibilities = value; }
        }

        public bool[] CastlingPossibilities
        {
            get { return castlingPosibilities; }
            private set { castlingPosibilities = value; }
        }

        public Dictionary<PieceColor, HashSet<ConstructorMenuPiece>> ConstructorPieces
        {
            get => menuPieces;
            private set => menuPieces = value;
        }

        public char[,] SimplifiedCells
        {
            get => simplifiedCells;
            private set => simplifiedCells = value;
        }

        public ConstructorCell[,] ConstructorCells
        {
            get => constructorCells;
            private set => constructorCells = value;
        }

        public void CreateCells()
        {
            ConstructorCells = new ConstructorCell[8, 8];
            SimplifiedCells = new char[8, 8];
            for (int row = 0; row < ConstructorCells.GetLength(0); row++)
            {
                for (int col = 0; col < ConstructorCells.GetLength(1); col++)
                {
                    ConstructorCells[row, col] = new ConstructorCell(row, col);
                    SimplifiedCells[row, col] = '.';
                }
            }
        }

        public void ImportPosition(Position position)
        {
            foreach (var piece in position.Pieces.Keys.SelectMany(color => position.Pieces[color]))
            {
                ConstructorCells[piece.Row, piece.Col].UpdatePiece(new ConstructorBoardPiece(piece.Row, piece.Col, piece.Color, piece.PieceType));
                var simplifiedPiece = GetSimplifedPiece(piece.Color, piece.PieceType);
                SimplifiedCells[piece.Row, piece.Col] = simplifiedPiece;
            }
            CastlingRights = position.CastlingRights;
            CastlingPossibilities = new bool[4];
            UpdateCastlingPosibilities();
            UpdateEnPassantPosibilities();
        }

        public void UpdateCastlingRights(bool[] castlingRights)
        {
            CastlingRights = (castlingRights[0], castlingRights[1], castlingRights[2], castlingRights[3]);
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
            if (constructorPiece == null)
            {
                SimplifiedCells[row, col] = '.';
            }
            else
            {
                SimplifiedCells[row, col] = GetSimplifedPiece(constructorPiece.Color, constructorPiece.PieceType);
            }
            if ((row == 0 || row == 7) && (col == 0 || col == 4 || col == 7))
            {
                UpdateCastlingPosibilities();
            }
            if (((row == 3 || row == 2 || row == 1) && TurnColor == PieceColor.White)
                || ((row == 4 || row == 5 || row == 6) && TurnColor == PieceColor.Black))
            {
                UpdateEnPassantPosibilities();
            }
        }

        public void UpdateTurnColor(PieceColor turnColor)
        {
            TurnColor = turnColor;
            UpdateEnPassantPosibilities();
        }

        public void UpdateEnPassantCoordinates(CellCoordinates? cellCoordinates)
        {
            EnPassantCoordinates = cellCoordinates.HasValue ? (cellCoordinates!.Value.Row, cellCoordinates.Value.Col) : null;
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

        private char GetSimplifedPiece(PieceColor color, PieceType pieceType)
        {
            var character = ' ';
            switch (pieceType)
            {
                case PieceType.Pawn:
                    character = 'p';
                    break;
                case PieceType.Knight:
                    character = 'n';
                    break;
                case PieceType.Bishop:
                    character = 'b';
                    break;
                case PieceType.Rook:
                    character = 'r';
                    break;
                case PieceType.Queen:
                    character = 'q';
                    break;
                case PieceType.Knook:
                    character = 'o';
                    break;
                case PieceType.King:
                    character = 'k';
                    break;
            }
            if (color == PieceColor.White)
            {
                character = Convert.ToChar(character - 32);
            }
            return character;
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

        private void UpdateCastlingPosibilities()
        {
            var isChanged = false;
            var condition = simplifiedCells[7, 4] == 'K' && simplifiedCells[7, 7] == 'R';
            if (CastlingPossibilities[0] != condition)
            {
                CastlingPossibilities[0] = condition;
                isChanged = true;
            }

            condition = simplifiedCells[7, 4] == 'K' && simplifiedCells[7, 0] == 'R';
            if (CastlingPossibilities[1] != condition)
            {
                CastlingPossibilities[1] = condition;
                isChanged = true;
            }

            condition = simplifiedCells[0, 4] == 'k' && simplifiedCells[0, 7] == 'r';
            if (CastlingPossibilities[2] != condition)
            {
                CastlingPossibilities[2] = condition;
                isChanged = true;
            }

            condition = simplifiedCells[0, 4] == 'k' && simplifiedCells[0, 0] == 'r';
            if (CastlingPossibilities[3] != condition)
            {
                CastlingPossibilities[3] = condition;
                isChanged = true;
            }

            if (isChanged)
            {
                CastlingPosibilitiesUpdate?.Invoke(null, EventArgs.Empty);
            }
        }

        private void UpdateEnPassantPosibilities()
        {
            EnPassantPossibilities.Clear();
            if (TurnColor == PieceColor.White)
            {
                var row = 3;
                for (int col = 0; col < SimplifiedCells.GetLength(0); col++)
                {
                    if (SimplifiedCells[row, col] == 'p'
                        && SimplifiedCells[row - 1, col] == '.'
                        && SimplifiedCells[row - 2, col] == '.')
                    {
                        EnPassantPossibilities.Add(new CellCoordinates(row - 1, col));
                    }
                }
            }
            else
            {
                var row = 4;
                for (int col = 0; col < SimplifiedCells.GetLength(0); col++)
                {
                    if (SimplifiedCells[row, col] == 'P'
                        && SimplifiedCells[row + 1, col] == '.'
                        && SimplifiedCells[row + 2, col] == '.')
                    {
                        EnPassantPossibilities.Add(new CellCoordinates(row + 1, col));
                    }
                }
            }
            EnPassantPosibilitiesUpdate?.Invoke(null, EventArgs.Empty);
        }

    }
}
