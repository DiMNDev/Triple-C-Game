namespace Chess_Final.PlayerManager;

using Chess_Final.DB_Manager;
using Player;

public class PlayerManager
{
    public static List<Player> OnlinePlayers { get; set; } = [];

    public static Player? SignIn(string username, string password)
    {
        // if username && password match in SQLite return true else false;
        if (username != null && password != null)
        {
            DB_Connect dB_Connect = new();
            Player? player = dB_Connect.AuthenticateUser((username, password));
            if (player != null)
            {
                Console.WriteLine($"SignIn: {player.Username}");
                OnlinePlayers.Add(player);
                return player;
            }
            else { return null; }
        }
        return null;
    }
    public static Player? SignUp(string username, string password, string confirm)
    {
        // if username && password match in SQLite return false else true;
        if (username != null && password != null && password == confirm)
        {
            Player player = new(username);
            DB_Connect dB_Connect = new();
            dB_Connect.InsertRecord(player, password);
            OnlinePlayers.Add(player);
            return player;
        }
        return null;
    }
    public static string GetUsername(Guid UUID)
    {
        DB_Connect dB_Connect = new();
        string username = dB_Connect.GetUsernameFromUUID(UUID);
        return username;
    }
    public static Player GetPlayer(Guid UUID)
    {
        DB_Connect dB_Connect = new();
        Player? data = dB_Connect.GetUserData(UUID);
        return data;
    }
}