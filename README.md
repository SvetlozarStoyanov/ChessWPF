# ChessWPF

Chess created in WPF.

Features:
1. Game
  - All chess rules implemented:
    + Castling
    + En Passant
    + Checks
    + Checkmates
    + Three move repetition leading to draw
    + Stalemates
  + Insufficient pieces to deliver checkmate cause draw
  - Clocks
  - New game button resetting board
  - Undo move button
  - Added new piece => Knook (moves like rook and knight combined)
  - Ability to export current position to popular chess websites by copying the FenAnnotation
  
2. Options:
  - Time Controls

3. Board Constructor
   - Creates custom positions
   - Save position (if valid) and start game from saved position
   - Functionalities:
     + Select and move piece
     + Delete piece
     + Add piece from menu
     + Validate current position and display error what is wrong
     + Set Castling rights
     + Set En Passant square
     + Set Turn color
     + Clear board button
     + Reset board button (resets to default position in chess)
     + Save board button - saves current position (if valid)
     + Load last saved position (resets to last saved position)
