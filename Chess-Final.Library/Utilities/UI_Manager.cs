namespace Chess_Final.UI_Manager;
using Chess_Final.Chess;
using Chess_Final.Generics;
using Chess_Final.Player;


public static class UI_Manager
{
    public static string CSS_ClassSelector(Guid UUID, string selection, int x, int y, Game game)
    {
        // Console.WriteLine(UUID);
        try
        {

            string selected = selection switch
            {
                "Chess_BG" => y % 2 == 0 ? x % 2 == 0 ? "background-color: black; color: white;" : "background-color: white; color: black;" : x % 2 != 0 ? "background-color: black; color: white;" : "background-color: white; color: black;",
                "Selected" => game?.CurrentPlayer?.SelectedPiece?.CurrentPosition == (((ChessCoordinate)x).ToString(), y) && UUID == game?.CurrentPlayer?.PlayerID ? "selected" : "",
                "Valid" => game?.CurrentPlayer?.SelectedPiece?.AllowedMovement?.Any(p => p.X == x && p.Y == y) == true && UUID == game?.CurrentPlayer?.PlayerID ? "validMove" : "",
                "Selected_Spec" => game?.CurrentPlayer?.SelectedPiece?.CurrentPosition == (((ChessCoordinate)x).ToString(), y) && UUID != game?.PlayerOne?.PlayerID && UUID != game?.PlayerTwo?.PlayerID ? "selected" : "",
                "Valid_Spec" => game?.CurrentPlayer?.SelectedPiece?.AllowedMovement?.Any(p => p.X == x && p.Y == y) == true && UUID != game?.PlayerOne?.PlayerID && UUID != game?.PlayerTwo?.PlayerID ? "validMove" : "",
                "Rotate_Opponent_Board" => UUID == game?.PlayerTwo?.PlayerID ? "rotateBoard" : ""
            };

            return selected;
        }
        catch (Exception)
        {

            return "";
        }
    }

    public static string GetImageSource(Game game, GamePiece gamePiece)
    {
        if (gamePiece is ChessPiece currentPiece)
        {
            pieceColor color = currentPiece.owner == Owner.Player ? pieceColor.White : pieceColor.Black;
            return currentPiece.Type switch
            {
                PieceType.pawn => $"/ChessPieces/{color}_Pawn.svg",
                PieceType.rook => $"/ChessPieces/{color}_Rook.svg",
                PieceType.bishop => $"/ChessPieces/{color}_Bishop.svg",
                PieceType.knight => $"/ChessPieces/{color}_Knight.svg",
                PieceType.queen => $"/ChessPieces/{color}_Queen.svg",
                PieceType.king => $"/ChessPieces/{color}_King.svg",
                _ => throw new InvalidGamePieceException()
            };
        }
        else
        {
            return "";
        }
    }
    private enum pieceColor
    {
        White,
        Black,
    }
}