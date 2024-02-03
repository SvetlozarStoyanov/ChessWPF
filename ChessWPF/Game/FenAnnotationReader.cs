using ChessWPF.Models.Data.Pieces;
using ChessWPF.Models.Data.Pieces.Enums;
using ChessWPF.Models.Positions;
using System;
using System.Collections.Generic;

namespace ChessWPF.Game
{
    public static class FenAnnotationReader
    {
        public static Position GetPosition(string fenAnnotation)
        {
            var position = new Position();
            var fenAnnotationSplit = fenAnnotation.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var rows = fenAnnotationSplit[0];
            position.Pieces = GetPieces(rows);
            position.TurnColor = fenAnnotationSplit[1] == "w" ? PieceColor.White : PieceColor.Black;
            position.CastlingRights = GetCastlingRights(fenAnnotationSplit[2]);
            position.EnPassantCoordinates = fenAnnotationSplit[3] != "-" ? GetEnPassantCoordinates(fenAnnotationSplit[4]) : null;
            position.HalfMoveCount = int.Parse(fenAnnotationSplit[4]);
            position.MoveNumber = int.Parse(fenAnnotationSplit[5]);
            position.FenAnnotation = fenAnnotation;
            return position;
        }

        private static ValueTuple<bool, bool, bool, bool> GetCastlingRights(string castlingRightsAnnotation)
        {
            var castlingRights = new ValueTuple<bool, bool, bool, bool>();

            if (castlingRightsAnnotation == "-")
            {
                return castlingRights;
            }
            foreach (char character in castlingRightsAnnotation)
            {
                switch (character)
                {
                    case 'K':
                        castlingRights.Item1 = true;
                        break;
                    case 'Q':
                        castlingRights.Item2 = true;
                        break;
                    case 'k':
                        castlingRights.Item3 = true;
                        break;
                    case 'q':
                        castlingRights.Item4 = true;
                        break;
                }
            }

            return castlingRights;
        }

        private static ValueTuple<int, int> GetEnPassantCoordinates(string enPassantAnnotation)
        {
            var coordinates = new ValueTuple<int, int>();

            coordinates.Item1 = enPassantAnnotation[1] - 97;
            coordinates.Item2 = (int)enPassantAnnotation[0] - 97;

            return coordinates;
        }

        private static Dictionary<PieceColor, List<Piece>> GetPieces(string rows)
        {
            var pieces = new Dictionary<PieceColor, List<Piece>>()
            {
                [PieceColor.White] = new List<Piece>(),
                [PieceColor.Black] = new List<Piece>()
            };

            var rowsSplit = rows.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int row = 0; row < rowsSplit.Length; row++)
            {
                var currRow = rowsSplit[row];
                for (int col = 0; col < currRow.Length; col++)
                {
                    if (char.IsNumber(currRow[col]))
                    {
                        col += currRow[col] - 48;
                    }
                    else
                    {
                        var piece = CreatePieceFromLetter(currRow[col], row, col);
                        pieces[piece.Color].Add(piece);
                    }
                }
            }
            return pieces;
        }

        private static Piece CreatePieceFromLetter(char letter, int row, int col)
        {
            var color = letter > 97 ? PieceColor.Black : PieceColor.White;
            var letterToLower = char.ToLower(letter);
            var pieceType = PieceType.Pawn;
            switch (letterToLower)
            {
                case 'k':
                    pieceType = PieceType.King;
                    break;
                case 'q':
                    pieceType = PieceType.Queen;
                    break;
                case 'o':
                    pieceType = PieceType.Knook;
                    break;
                case 'r':
                    pieceType = PieceType.Rook;
                    break;
                case 'b':
                    pieceType = PieceType.Bishop;
                    break;
                case 'n':
                    pieceType = PieceType.Knight;
                    break;
                case 'p':
                    pieceType = PieceType.Pawn;
                    break;
            }

            var piece = PieceConstructor.ConstructPieceByType(
                pieceType,
                color,
                row,
                col
                );
            return piece;
        }
    }
}
