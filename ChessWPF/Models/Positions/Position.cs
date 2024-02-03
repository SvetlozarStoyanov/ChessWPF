using ChessWPF.Models.Data.Board;
using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
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
        private ValueTuple<bool,bool,bool,bool> castlingRights;
        private ValueTuple<int, int>? enPassantCoordinates;
        private Dictionary<PieceColor, List<Piece>> pieces;

        public Position()
        {
            
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

        public ValueTuple<int, int>? EnPassantCoordinates
        {
            get => enPassantCoordinates;
            set => enPassantCoordinates = value;
        }

        public ValueTuple<bool, bool, bool, bool> CastlingRights
        {
            get { return castlingRights; }
            set { castlingRights = value; }
        }

        public Dictionary<PieceColor, List<Piece>> Pieces
        {
            get => pieces;
            set => pieces = value;
        }

    }
}
