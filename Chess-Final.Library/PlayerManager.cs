namespace Chess_Final.PlayerManager;

using Chess_Final.DB_Manager;
using Player;

public class PlayerManager
{
    public static PlayerManager Instance;
    public List<Player> OnlinePlayers { get; set; }

    public Player? SignIn(string username, string password)
    {
        // if username && password match in SQLite return true else false;
        if(username != null && password != null) {
            DB_Connect dB_Connect = new();
            Player player = dB_Connect.LoadUserData();
        return player;
        }
        return null;
    }
    public Player? SignUp(string username, string password, string confirm)
    {
        // if username && password match in SQLite return false else true;
        if(username != null && password != null && password == confirm) {
            Player player = new(username);
            DB_Connect dB_Connect = new();
            dB_Connect.InsertRecord(player);
        return player;
        }
        return null;
    }
}