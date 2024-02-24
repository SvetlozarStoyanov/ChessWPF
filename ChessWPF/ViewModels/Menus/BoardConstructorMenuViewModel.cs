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
        private bool saveIsEnabled = true;
        private string? positionErrorText;
        private PieceColor selectedTurnColor;
        private CellCoordinates? selectedEnPassantCoordinates;
        private ObservableCollection<PieceColor> turnColors;
        private ObservableCollection<CellCoordinates?> enPassantPossibilities;
        private ObservableCollection<bool> castlingRights;
        private ObservableCollection<bool> castlingPossibilities;

        public BoardConstructorMenuViewModel(CellCoordinates? enPassantCoordinates,
            bool[] castlingRights,
            bool[] castlingPossibilities,
            HashSet<CellCoordinates?> enPassantPossibilities,
            PieceColor turnColor)
        {
            selectedEnPassantCoordinates = enPassantCoordinates;
            CastlingRights = new ObservableCollection<bool>(castlingRights);
            CastlingPossibilities = new ObservableCollection<bool>(castlingPossibilities);
            TurnColors = new ObservableCollection<PieceColor>()
            {
                PieceColor.White,
                PieceColor.Black
            };
            SelectedTurnColor = turnColor;
            SetCastlingRightsCommand = new SetCastlingRightsCommand(this);
            ResetBoardToDefaultCommand = new ResetBoardToDefaultCommand(this);
            LoadSavedPositionCommand = new LoadSavedPositionCommand(this);
            ClearBoardCommand = new ClearBoardCommand(this);
            SavePositionCommand = new SavePositionCommand(this);
            InitializeEnPassantPossibilities(enPassantPossibilities);
        }

        public event UpdateTurnColorEventHandler TurnColorUpdate;
        public delegate void UpdateTurnColorEventHandler(object? sender, TurnColorChangedEventArgs e);
        public event UpdateCastlingRightsEventHandler CastlingRightsUpdate;
        public delegate void UpdateCastlingRightsEventHandler(object? sender, EventArgs e);
        public event UpdateEnPassantCoordinatesEventHandler EnPassantCoordinatesUpdate;
        public delegate void UpdateEnPassantCoordinatesEventHandler(object? sender, EnPassantCoordinatesChangedEventArgs e);
        public event ResetBoardEventHandler ResetBoardToDefault;
        public delegate void ResetBoardEventHandler(object? sender, EventArgs e);
        public event LoadSavedPositionEventHandler LoadLastSavedPosition;
        public delegate void LoadSavedPositionEventHandler(object? sender, EventArgs e);
        public event ClearBoardEventHandler ClearBoard;
        public delegate void ClearBoardEventHandler(object? sender, EventArgs e);
        public event SavePositionEventHandler SaveCurrentPosition;
        public delegate void SavePositionEventHandler(object? sender, EventArgs e);

        public bool SaveIsEnabled
        {
            get => saveIsEnabled;
            set
            {
                saveIsEnabled = value;
                OnPropertyChanged(nameof(SaveIsEnabled));
            }
        }


        public string? PositionErrorText
        {
            get => positionErrorText;
            set 
            { 
                positionErrorText = value;
                OnPropertyChanged(nameof(PositionErrorText));
            }
        }

        public PieceColor SelectedTurnColor
        {
            get => selectedTurnColor;
            set
            {
                if (value != selectedTurnColor)
                {
                    selectedTurnColor = value;
                    OnPropertyChanged(nameof(SelectedTurnColor));
                    TurnColorUpdate?.Invoke(null, new TurnColorChangedEventArgs(selectedTurnColor));
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
            private set => castlingRights = value;
        }

        public ObservableCollection<bool> CastlingPossibilities
        {
            get => castlingPossibilities;
            private set => castlingPossibilities = value;
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
        public ICommand SetCastlingRightsCommand { get; init; }
        public ICommand SetEnPassantCoordinatesCommand { get; init; }
        public ICommand ResetBoardToDefaultCommand { get; init; }
        public ICommand LoadSavedPositionCommand { get; init; }
        public ICommand ClearBoardCommand { get; init; }
        public ICommand SavePositionCommand { get; init; }

        public void UpdateCastlingRights()
        {
            CastlingRightsUpdate(this, EventArgs.Empty);
        }

        public void UpdateCastlingRightsBackend(bool[] castlingRights)
        {
            for (int i = 0; i < CastlingRights.Count; i++)
            {
                CastlingRights[i] = castlingRights[i];
            }
        }

        public void UpdateCastlingPossiblities(bool[] castlingPossibilities)
        {
            for (int i = 0; i < CastlingPossibilities.Count; i++)
            {
                CastlingPossibilities[i] = castlingPossibilities[i];
            }
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
            ResetBoardToDefault?.Invoke(this, EventArgs.Empty);
        }

        public void ClearPieces()
        {
            //SelectedEnPassantCoordinates = null;
            ClearBoard?.Invoke(null, EventArgs.Empty);
        }

        public void SavePosition()
        {
            SaveCurrentPosition?.Invoke(null, EventArgs.Empty);
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

        public void LoadSavedPosition()
        {
            LoadLastSavedPosition?.Invoke(null,EventArgs.Empty);
        }
    }
}
