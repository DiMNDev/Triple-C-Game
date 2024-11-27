namespace Chess_Final.UI_Manager;

using Chess_Final.Generics;
using Chess_Final.Player;


public static class UI_Manager
{
    public static List<string> PlayerOneStyles = ["Chess_BG"];
    public static List<string> PlayerTwoStyles = ["Chess_BG"];
    public static List<string> SpectatorStyles = [];
    public static event Action UpdateUI;

    public static void addStyleTo(Player player, string selection, int x, int y, Game game)
    {
        string style = CSS_ClassSelector(player, selection, x, y, game);
        if (player == game.PlayerOne)
        {
            PlayerOneStyles.Add(style);
        }
        else if (player == game.PlayerTwo)
        {
            PlayerTwoStyles.Add(style);
        }
        UpdateUI?.Invoke();

    }

    public static void removeStyleFrom(Player player, string selection, int x, int y, Game game)
    {

        string style = CSS_ClassSelector(player, selection, x, y, game);
        if (player == game.PlayerOne)
        {
            PlayerOneStyles.Remove(style);
        }
        else if (player == game.PlayerTwo)
        {
            PlayerTwoStyles.Remove(style);
        }
        UpdateUI?.Invoke();
    }
    public static string CSS_ClassSelector(Player player, string selection, int x, int y, Game game)
    {
        // Console.WriteLine($"Update UI: {player.Username}");
        // Console.WriteLine($"SelecedPiece: {player.SelectedPiece?.AllowedMovement[0].Y}");
        // Console.WriteLine($"SelecedPiece: {game?.CurrentPlayer?.SelectedPiece?.AllowedMovement[0].Y}");
        Enum.TryParse<ChessCoordinate>(player?.SelectedPiece?.CurrentPosition.X, out ChessCoordinate currentX);
        if (selection == "Selected" && player?.SelectedPiece?.CurrentPosition.Y == y && (int)currentX == x)
        {
            Console.WriteLine($"SelectedStyle: ({(int)currentX}, {y})");
            Console.WriteLine($"Player: {player.Username} X:{x} Y:{y}");
        }
        try
        {

            string selected = selection switch
            {
                "Chess_BG" => y % 2 == 0 ? x % 2 == 0 ? "background-color: black; color: white;" : "background-color: white; color: black;" : x % 2 != 0 ? "background-color: black; color: white;" : "background-color: white; color: black;",
                // "Selected" => player?.SelectedPiece?.CurrentPosition.X == ((ChessCoordinate)x).ToString() && player?.SelectedPiece?.CurrentPosition.Y == y ? "selected" : "",
                "Selected" => game.CurrentPlayer.SelectedPiece.CurrentPosition == (((ChessCoordinate)x).ToString(), y) ? "selected" : "",
                // "Select" => player == game.CurrentPlayer && player.SelectedPiece.CurrentPosition.X == ((ChessCoordinate)x).ToString() && player?.SelectedPiece?.CurrentPosition.Y == y ? "selected" : "",
                "Valid" => game.CurrentPlayer.SelectedPiece.AllowedMovement?.Any(p => p.X == x && p.Y == y) == true ? "validMove" : "",
                // "Valids" => player == game.CurrentPlayer && player.SelectedPiece.AllowedMovement?.Any(p => p.X == x && p.Y == y) == true ? "validMove" : "",
            };

            return selected;
        }
        catch (Exception)
        {

            return "";
        }
    }
}


/*
                            class="GridCell @(game?.CurrentPlayer == player ? UI_Manager.CSS_ClassSelector(player!,"
                            Chess_Selection", LocalX, LocalY, game) : "" ) @(UI_Manager.CSS_ClassSelector(player!,"Valid_Moves",
                            LocalX, LocalY, game))" */