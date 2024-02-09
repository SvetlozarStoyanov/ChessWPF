using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Cells;
using ChessWPF.Models.Pieces.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class BoardConstructorMenuViewModel : ViewModelBase
    {
        private PieceColor selectedTurnColor;
        private CellCoordinates? selectedEnPassantCoordinates;
        private ObservableCollection<PieceColor> turnColors;
        private ObservableCollection<CellCoordinates?> enPassantPossibilities;
        private ObservableCollection<bool> castlingRights;
        private ObservableCollection<bool> castlingPossibilities;

        public BoardConstructorMenuViewModel(bool[] castlingRights,
            bool[] castlingPosibilities,
            HashSet<CellCoordinates?> enPassantPossibilities)
        {
            CastlingRights = new ObservableCollection<bool>(castlingRights);
            CastlingPossibilities = new ObservableCollection<bool>(castlingPossibilities);
            TurnColors = new ObservableCollection<PieceColor>()
            {
                PieceColor.White,
                PieceColor.Black
            };
            SetCastlingRightsCommand = new SetCastlingRightsCommand(this);
            ResetBoardToDefaultCommand = new ResetBoardToDefaultCommand(this);
            InitializeEnPassantPossibilities(enPassantPossibilities);
        }

        public event UpdateTurnColorEventHandler TurnColorUpdate;
        public delegate void UpdateTurnColorEventHandler(object? sender, TurnColorChangedEventArgs e);
        public event UpdateCastlingRightsEventHandler CastlingRightsUpdate;
        public delegate void UpdateCastlingRightsEventHandler(object? sender, EventArgs e);
        public event UpdateEnPassantCoordinatesEventHandler EnPassantCoordinatesUpdate;
        public delegate void UpdateEnPassantCoordinatesEventHandler(object? sender, EnPassantCoordinatesChangedEventArgs e);
        public event ResetBoardEventHandler ResetToDefault;
        public delegate void ResetBoardEventHandler(object? sender, EventArgs e);

        public PieceColor SelectedTurnColor
        {
            get => selectedTurnColor;
            set
            {
                if (value != selectedTurnColor)
                {
                    selectedTurnColor = value;
                    OnPropertyChanged(nameof(SelectedTurnColor));
                    TurnColorUpdate(null, new TurnColorChangedEventArgs(selectedTurnColor));
                }
            }
        }

        public CellCoordinates? SelectedEnPassantCoordinates
        {
            get { return selectedEnPassantCoordinates; }
            set
            {
                if (value.HasValue && value!.Value.Row == -1 && value.Value.Row == -1)
                {
                    selectedEnPassantCoordinates = null;
                }
                else
                {
                    selectedEnPassantCoordinates = value;
                }
                OnPropertyChanged(nameof(SelectedEnPassantCoordinates));
                if (selectedEnPassantCoordinates.HasValue)
                {
                    EnPassantCoordinatesUpdate(null, new EnPassantCoordinatesChangedEventArgs(selectedEnPassantCoordinates.Value));
                }
                else
                {
                    EnPassantCoordinatesUpdate(null, new EnPassantCoordinatesChangedEventArgs(null));
                }
            }
        }

        public ObservableCollection<bool> CastlingRights
        {
            get => castlingRights;
            private set
            {
                castlingRights = value;
                OnPropertyChanged(nameof(CastlingRights));
            }
        }

        public ObservableCollection<bool> CastlingPossibilities
        {
            get => castlingPossibilities;
            private set
            {
                castlingPossibilities = value;
                OnPropertyChanged(nameof(CastlingPossibilities));
            }
        }

        public ObservableCollection<PieceColor> TurnColors
        {
            get => turnColors;
            private set => turnColors = value;
        }

        public ObservableCollection<CellCoordinates?> EnPassantPossibilities
        {
            get => enPassantPossibilities;
            private set => enPassantPossibilities = value;
        }

        public ICommand SetTurnColorCommand { get; init; }
        public ICommand ClearBoardCommand { get; init; }
        public ICommand SetCastlingRightsCommand { get; init; }
        public ICommand SetEnPassantCoordinatesCommand { get; init; }
        public ICommand ResetBoardToDefaultCommand { get; init; }
        public ICommand ClearBoardCommand { get; init; }

        public void UpdateCastlingRights()
        {
            CastlingRightsUpdate(this, EventArgs.Empty);
        }

        public void UpdateCastlingPosiblities(bool[] castlingPosibilities)
        {
            CastlingPossibilities = castlingPosibilities;
        }

        public void UpdateEnPassantPosibilities(HashSet<CellCoordinates?> enPassantPosibilities)
        {
            foreach (var item in enPassantPosibilities)
            {
                if (!EnPassantPossibilities.Contains(item))
                {
                    try
                    {
                        EnPassantPossibilities.Add(item);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            var cellCoordinatesToRemove = new HashSet<CellCoordinates?>();
            foreach (var item in EnPassantPossibilities)
            {
                if (item!.Value.Row != -1 && !enPassantPosibilities.Contains(item))
                {
                    cellCoordinatesToRemove.Add(item);
                }
            }

            foreach (var item in cellCoordinatesToRemove)
            {
                try
                {
                    EnPassantPossibilities.Remove(item);
                }
                catch (Exception ex)
                {

                }
            }

            if (SelectedEnPassantCoordinates != null && !EnPassantPossibilities.Any(cc => cc.Equals(SelectedEnPassantCoordinates)))
            {
                SelectedEnPassantCoordinates = null;
            }
        }

        public void ResetBoard()
        {
            ResetToDefault?.Invoke(this, EventArgs.Empty);
        }

        private void InitializeEnPassantPossibilities(HashSet<CellCoordinates?> enPassantPosibilities)
        {
            EnPassantPossibilities = new ObservableCollection<CellCoordinates?>()
            {
                new CellCoordinates(-1,-1)
            };
            foreach (var item in enPassantPosibilities)
            {
                try
                {
                    EnPassantPossibilities.Add(item);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }
    }
}
