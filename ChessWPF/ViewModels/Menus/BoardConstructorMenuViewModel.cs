using ChessWPF.Commands;
using ChessWPF.HelperClasses.CustomEventArgs;
using ChessWPF.Models.Data.Pieces.Enums;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChessWPF.ViewModels
{
    public class BoardConstructorMenuViewModel : ViewModelBase
    {
        private PieceColor selectedTurnColor;
        private ObservableCollection<PieceColor> turnColors;
        private bool[] castlingRights;
        private bool[] castlingPosibilities;

        public BoardConstructorMenuViewModel(bool[] castlingRights, bool[] castlingPosibilities)
        {
            CastlingRights = castlingRights;
            CastlingPosibilities = castlingPosibilities;
            TurnColors = new ObservableCollection<PieceColor>()
            {
                PieceColor.White,
                PieceColor.Black
            };
            SetCastlingRightsCommand = new SetCastlingRightsCommand(this);
        }

        public event UpdateTurnColorEventHandler TurnColorUpdate;
        public delegate void UpdateTurnColorEventHandler(object? sender, TurnColorChangedEventArgs e);
        public event UpdateCastlingRightsEventHandler CastlingRightsUpdate;
        public delegate void UpdateCastlingRightsEventHandler(object? sender, EventArgs e);


        public PieceColor SelectedTurnColor
        {
            get => selectedTurnColor;
            set
            {
                if (value != selectedTurnColor)
                {
                    selectedTurnColor = value;
                    OnPropertyChanged(nameof(SelectedTurnColor));
                    TurnColorUpdate(null,new TurnColorChangedEventArgs(selectedTurnColor));
                }
            }
        }

        public bool[] CastlingRights
        {
            get => castlingRights;
            private set
            {
                castlingRights = value;
                OnPropertyChanged(nameof(CastlingRights));
            }
        }

        public bool[] CastlingPosibilities
        {
            get => castlingPosibilities;
            private set
            {
                castlingPosibilities = value;
                OnPropertyChanged(nameof(CastlingPosibilities));
            }
        }

        public ObservableCollection<PieceColor> TurnColors
        {
            get => turnColors;
            private set => turnColors = value;
        }

        public ICommand SetTurnColorCommand { get; init; }
        public ICommand ClearBoardCommand { get; init; }
        public ICommand SetCastlingRightsCommand { get; init; }
        public ICommand SetEnPassantSquareCommand { get; init; }


        public void UpdateCastlingRights()
        {
            CastlingRightsUpdate(this, EventArgs.Empty);
        }

        public void UpdateCastlingPosiblities(bool[] castlingPosibilities)
        {
            CastlingPosibilities = castlingPosibilities;
        }
    }
}
