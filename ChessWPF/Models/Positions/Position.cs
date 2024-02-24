using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;

namespace ChessWPF.Models.Positions
{
    public sealed class Position
    {
        private int halfMoveCount;
        private int moveNumber;
        private string fenAnnotation;
        private PieceColor turnColor;
        private bool[] castlingRights;
        private CellCoordinates? enPassantCoordinates;
        private Dictionary<PieceColor, List<Piece>> pieces;
        private char[,] simplifiedCells;

        public Position()
        {

        }

        public Position(char[,] simplifiedCells,
            PieceColor turnColor,
            string fenAnnotation,
            bool[] castlingRights,
            CellCoordinates? enPassantCoordinates,
            int halfMoveCount = 0,
            int moveNumber = 1)
        {
            SimplifiedCells = simplifiedCells;
            TurnColor = turnColor;
            FenAnnotation = fenAnnotation;
            CastlingRights = castlingRights;
            EnPassantCoordinates = enPassantCoordinates;
            HalfMoveCount = halfMoveCount;
            MoveNumber = moveNumber;
        }

        public int HalfMoveCount
        {
            get => halfMoveCount;
            set => halfMoveCount = value;
        }

        public int MoveNumber
        {
            get => moveNumber;
            set => moveNumber = value;
        }

        public string FenAnnotation
        {
            get => fenAnnotation;
            set => fenAnnotation = value;
        }

        public PieceColor TurnColor
        {
            get => turnColor;
            set => turnColor = value;
        }

        public CellCoordinates? EnPassantCoordinates
        {
            get => enPassantCoordinates;
            set => enPassantCoordinates = value;
        }

        public bool[] CastlingRights
        {
            get { return castlingRights; }
            set { castlingRights = value; }
        }

        public Dictionary<PieceColor, List<Piece>> Pieces
        {
            get => pieces;
            set => pieces = value;
        }

        public char[,] SimplifiedCells
        {
            get { return simplifiedCells; }
            set { simplifiedCells = value; }
        }

    }
}
