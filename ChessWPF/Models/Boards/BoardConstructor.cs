using ChessWPF.Contracts.Pieces;
using ChessWPF.Game;
using ChessWPF.HelperClasses.CustomEventArgs;
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

        public event UpdateCastlingPosibilitiesEventHandler CastlingPossibilitiesUpdate;
        public delegate void UpdateCastlingPosibilitiesEventHandler(object? sender, EventArgs e);
        public event UpdateCastlingRightsEventHandler CastlingRightsUpdate;
        public delegate void UpdateCastlingRightsEventHandler(object? sender, UpdateCastlingRightsEventArgs e);
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

        public void ResetBoardToDefault()
        {
            var position = PositionCreator.CreateDefaultPosition();
            ClearAllPieces();
            ImportPosition(position);
            UpdateTurnColor(position.TurnColor);
            UpdateCastlingRightsBackend();
            CastlingPossibilitiesUpdate?.Invoke(null, EventArgs.Empty);
        }

        public void ClearBoard()
        {
            ClearAllPieces();
            ClearCastlingPossibilities();
            CastlingRights = (false, false, false, false);
            ClearEnPassantPossibilities();
            UpdateCastlingRightsBackend();
        }

        public void ClearAllPieces()
        {
            for (int row = 0; row < ConstructorCells.GetLength(0); row++)
            {
                for (int col = 0; col < ConstructorCells.GetLength(1); col++)
                {
                    ConstructorCells[row, col].UpdatePiece(null);
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
            UpdateTurnColor(position.TurnColor);
            UpdateCastlingPossibilities();
            UpdateEnPassantPossibilities();
        }

        public Position ExportPosition()
        {
            var position = new Position();
            position.Pieces = FindPieces();

            return position;
        }

        public void UpdateCastlingRightsFromUI(bool[] castlingRights)
        {
            CastlingRights = (castlingRights[0], castlingRights[1], castlingRights[2], castlingRights[3]);
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
                UpdateCastlingPossibilities();
            }
            if (((row == 3 || row == 2 || row == 1) && TurnColor == PieceColor.White)
                || ((row == 4 || row == 5 || row == 6) && TurnColor == PieceColor.Black))
            {
                UpdateEnPassantPossibilities();
            }
        }

        public void UpdateTurnColor(PieceColor turnColor)
        {
            TurnColor = turnColor;
            UpdateEnPassantPossibilities();
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

        private void UpdateCastlingPossibilities()
        {
            var isChanged = false;
            var condition = simplifiedCells[7, 4] == 'K' && simplifiedCells[7, 7] == 'R';
            if (CastlingPossibilities[0] != condition)
            {
                isChanged = true;
            }
            CastlingPossibilities[0] = condition;

            condition = simplifiedCells[7, 4] == 'K' && simplifiedCells[7, 0] == 'R';
            if (CastlingPossibilities[1] != condition)
            {
                isChanged = true;
            }
            CastlingPossibilities[1] = condition;

            condition = simplifiedCells[0, 4] == 'k' && simplifiedCells[0, 7] == 'r';
            if (CastlingPossibilities[2] != condition)
            {
                isChanged = true;
            }
            CastlingPossibilities[2] = condition;

            condition = simplifiedCells[0, 4] == 'k' && simplifiedCells[0, 0] == 'r';
            if (CastlingPossibilities[3] != condition)
            {
                isChanged = true;
            }
            CastlingPossibilities[3] = condition;

            if (isChanged)
            {
                CastlingPossibilitiesUpdate?.Invoke(null, EventArgs.Empty);
            }
        }

        private void UpdateCastlingRightsBackend()
        {
            CastlingRightsUpdate?.Invoke(
                null,
                new UpdateCastlingRightsEventArgs(new bool[4]
            {
                CastlingRights.Item1,
                CastlingRights.Item2,
                CastlingRights.Item3,
                CastlingRights.Item4
            }));
        }

        private void UpdateEnPassantPossibilities()
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

        private void ClearCastlingPossibilities()
        {
            Array.Fill(CastlingPossibilities, false);
            CastlingPossibilitiesUpdate?.Invoke(null, EventArgs.Empty);
        }

        private void ClearEnPassantPossibilities()
        {
            EnPassantPossibilities.Clear();
            EnPassantPosibilitiesUpdate?.Invoke(null, EventArgs.Empty);
        }
    }
}
