namespace Chess_Final.UI_Manager;

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
            };

            return selected;
        }
        catch (Exception)
        {

            return "";
        }
    }
}