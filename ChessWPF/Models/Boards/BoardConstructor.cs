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
        private ValueTuple<int, int>? enPassantCoordinates;
        private ValueTuple<bool, bool, bool, bool> castlingRights;
        private bool[] castlingPosibilities;
        private Dictionary<PieceColor, HashSet<ConstructorMenuPiece>> menuPieces;
        private char[,] simplifiedCells;
        private ConstructorCell[,] constructorCells;

        public BoardConstructor()
        {
            CreateCells();
            CreateConstructorPieces();
        }

        public event UpdateCastlingPosibilitiesEventHandler CastlingPosibilitiesUpdate;
        public delegate void UpdateCastlingPosibilitiesEventHandler(object? sender, EventArgs e);

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
        public bool[] CastlingPosibilities
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

        private void UpdateCastlingPosibilities()
        {
            var isChanged = false;
            var condition = simplifiedCells[7, 4] == 'K' && simplifiedCells[7, 7] == 'R';
            if (CastlingPosibilities[0] != condition)
            {
                CastlingPosibilities[0] = condition;
                isChanged = true;
            }

            condition = simplifiedCells[7, 4] == 'K' && simplifiedCells[7, 7] == 'R';
            if (CastlingPosibilities[1] != condition)
            {
                CastlingPosibilities[1] = condition;
                isChanged = true;
            }

            condition = simplifiedCells[0, 4] == 'k' && simplifiedCells[0, 7] == 'r';
            if (CastlingPosibilities[2] != condition)
            {
                CastlingPosibilities[2] = condition;
                isChanged = true;
            }

            condition = simplifiedCells[0, 4] == 'k' && simplifiedCells[0, 0] == 'r';
            if (CastlingPosibilities[3] != condition)
            {
                CastlingPosibilities[3] = condition;
                isChanged = true;
            }

            if (isChanged)
            {
                CastlingPosibilitiesUpdate?.Invoke(null, EventArgs.Empty);
            }
        }


    }
}
