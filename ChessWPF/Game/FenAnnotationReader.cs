using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces;
using ChessWPF.Models.Pieces.Enums;
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
            var piecesAndSimplifiedCells = GetPiecesAndSimplifiedCells(rows);
            position.Pieces = piecesAndSimplifiedCells.Item1;
            position.SimplifiedCells = piecesAndSimplifiedCells.Item2;
            position.TurnColor = fenAnnotationSplit[1] == "w" ? PieceColor.White : PieceColor.Black;
            position.CastlingRights = GetCastlingRights(fenAnnotationSplit[2]);
            position.EnPassantCoordinates = fenAnnotationSplit[3] != "-" ? GetEnPassantCoordinatesFromAnnotation(fenAnnotationSplit[3]) : null;
            position.HalfMoveCount = int.Parse(fenAnnotationSplit[4]);
            position.MoveNumber = int.Parse(fenAnnotationSplit[5]);
            position.FenAnnotation = fenAnnotation;
            return position;
        }

        private static bool[] GetCastlingRights(string castlingRightsAnnotation)
        {
            var castlingRights = new bool[4];

            if (castlingRightsAnnotation == "-")
            {
                return castlingRights;
            }
            foreach (char character in castlingRightsAnnotation)
            {
                switch (character)
                {
                    case 'K':
                        castlingRights[0] = true;
                        break;
                    case 'Q':
                        castlingRights[1] = true;
                        break;
                    case 'k':
                        castlingRights[2] = true;
                        break;
                    case 'q':
                        castlingRights[3] = true;
                        break;
                }
            }

            return castlingRights;
        }

        private static CellCoordinates GetEnPassantCoordinatesFromAnnotation(string enPassantAnnotation)
        {
            var coordinates = new CellCoordinates(8 - (int)(enPassantAnnotation[1] - 48), ((int)(enPassantAnnotation[0] - 97)));
            return coordinates;
        }

        private static ValueTuple<Dictionary<PieceColor, List<Piece>>, char[,]> GetPiecesAndSimplifiedCells(string rows)
        {
            var pieces = new Dictionary<PieceColor, List<Piece>>()
            {
                [PieceColor.White] = new List<Piece>(),
                [PieceColor.Black] = new List<Piece>()
            };
            var simplifiedCells = new char[8, 8];

            var rowsSplit = rows.Split('/', StringSplitOptions.RemoveEmptyEntries);
            for (int row = 0; row < rowsSplit.Length; row++)
            {
                var currRow = rowsSplit[row];
                var col = 0;
                for (int rowIndex = 0; rowIndex < currRow.Length; rowIndex++)
                {
                    if (char.IsNumber(currRow[rowIndex]))
                    {
                        var emptyCols = currRow[rowIndex] - 48;
                        while (emptyCols > 0)
                        {
                            simplifiedCells[row, col] = '.';
                            col++;
                            emptyCols--;
                        }
                    }
                    else
                    {
                        simplifiedCells[row, col] = currRow[rowIndex];
                        var piece = CreatePieceFromLetter(currRow[rowIndex], row, col);
                        pieces[piece.Color].Add(piece);
                        col++;
                    }
                }
            }
            return (pieces, simplifiedCells);
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

            var piece = PieceCreator.CreatePieceByProperties(
                pieceType,
                color,
                row,
                col
                );
            return piece;
        }
    }
}
