# ChessWPF

Chess created in WPF.

  ![move-and-castling](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Move%20and%20castling.gif)
  
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


- Ability to export current position to popular chess websites by copying the FenAnnotation
  
  ![fen-compatibility](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Fen%20compatibility.gif)

- Pawn promotions

  ![promotion](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Promotion.gif)

- Cancelling promotion
  
  ![promotion-cancel](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Promotion%20cancellation.gif)
  
- Added new piece => Knook (moves like rook and knight combined)
    
  ![knook](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Knook.gif)

2. Board Constructor
   - Creates custom positions

     ![place-piece](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Board%20constructor%20place%20piece.gif)
     
   - Save position (if valid) and start game from saved position

     ![save-button](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Save%20button.gif)

   - Select and move piece
   - Delete piece
   - Add piece from menu

   - Set Castling rights
   - Set En Passant square
   - Set Turn color
   - Clear board button

     ![clear-board-button](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Clear%20board.gif)
     
   - Reset board button (resets to default position in chess)

      ![reset-board-button](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Reset%20board.gif)
     
   - Load last saved position (resets to last saved position)

     ![load-last-saved-button](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Load%20last%20saved.gif)
     
   - Validate current position and display error what is wrong
 
      ![validation](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Validation.gif)
    
3. Options
  - Time Controls

   ![time-control](https://github.com/SvetlozarStoyanov/ChessWPF/blob/6c4bd3146a650b30b8ec51f5226a7e4c5d378efb/ChessWPF/Documentation/Time%20control.gif)


