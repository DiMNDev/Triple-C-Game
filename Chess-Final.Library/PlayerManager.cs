namespace Chess_Final.PlayerManager;
using Player;

public class PlayerManager
{
    public static PlayerManager Instance;
    public List<Player> OnlinePlayers { get; set; }

    public Player? SignIn(string username, string password)
    {
        // if username && password match in SQLite return true else false;
        return null;
    }
    public Player? SignUp(string username, string password, string confirm)
    {
        // if username && password match in SQLite return false else true;
        return null;
    }
}