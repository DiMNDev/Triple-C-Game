namespace Chess_Final.DB_Manager;
using Chess_Final.Generics;
using SQLite;
using Player;
using Chess_Final.PasswordManager;
using SQLitePCL;

public class DB_Connect
{
    SQLiteConnection? _connection;

    public string ConnectionString { get; private set; } = "TC_DB.sqlite3";

    // Consider returning status(string) tuple from all methods
    public DB_Connect()
    {
        IntializeConnection();
    }
    private void InitializeTables()
    {
        _connection?.CreateTable<PD_Table>();
        _connection?.CreateTable<Auth_Table>();
    }
    public void IntializeConnection()
    {
        SQLiteConnectionString options = new SQLiteConnectionString(ConnectionString, false);
        _connection = new SQLiteConnection(options);
        InitializeTables();
    }
    public void CreateAuthForUser(Guid UUID, string username, string password)
    {
        //hash password before storing
        string hash = PasswordManager.HashPassword(password);
        Auth_Table auth = new() { PlayerID = UUID, Password = hash };
        _connection?.Insert(auth);
    }
    public bool AccountExists(string username)
    {
        PD_Table user = _connection?.Table<PD_Table>().FirstOrDefault(p => p.Username == username);
        Console.WriteLine(user);
        if (user != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool InsertRecord(Player player, string password)
    {
        // create a new record
        if (AccountExists(player.Username))
        {
            return false;
        }
        CreateAuthForUser(player.PlayerID, player.Username, password);
        PD_Table newRecord = new() { PlayerID = player.PlayerID, Username = player.Username, Losses = player.Losses, Wins = player.Wins };
        try
        {
            var result = _connection?.Insert(newRecord);
            if (result != null && result == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception err)
        {
            Console.WriteLine(err);
            return false;
        }
    }
    public void UpdateRecord(Player player)
    {
        var data = _connection.Find<PD_Table>(player.PlayerID);
        _connection.Update(data);
    }
    public void DeleteRecord(string username)
    {
        // Get Player UUID
        Guid UserToDelete = GetRecord(username).PlayerID;
        // Delete from PD_Table
        _connection.Delete<PD_Table>(UserToDelete);
        // Delete from Auth_Table
        _connection.Delete<Auth_Table>(UserToDelete);
    }
    private Guid? GetPlayerUUID(string username)
    {
        try
        {
            PD_Table record = GetRecord(username);
            return record.PlayerID;
        }
        catch (Exception)
        {

            return null;
        }
    }
    public PD_Table? GetRecord(string username)
    {
        PD_Table? data = _connection?.Table<PD_Table>().FirstOrDefault(p => p.Username == username);
        return data ?? null;
    }
    public bool VerifyAccount(string username, string password)
    {
        // get hash from player record for verification
        Guid? UUID = GetPlayerUUID(username);
        if (UUID != null)
        {
            string hash = GetUserAuth(UUID!);
            // verify hash
            return PasswordManager.VerifyPassword(password, hash);
        }
        else
        {
            return false;
        }

    }
    internal Player? AuthenticateUser((string username, string password) formData)
    {
        // if guid matches create new player
        bool valid = VerifyAccount(formData.username, formData.password);
        if (valid)
        {
            PD_Table data = GetRecord(formData.username);
            Player User = new(data.Username, data.PlayerID);
            return User;
        }
        else
        {
            return null;
        }
    }
    private string GetUserAuth(Guid? UUID)
    {
        Auth_Table? data = _connection?.Table<Auth_Table>().FirstOrDefault(id => id.PlayerID == UUID);
        return data.Password;
    }
    internal string? GetUsernameFromUUID(Guid UUID)
    {
        PD_Table? data = _connection?.Table<PD_Table>().FirstOrDefault(p => p.PlayerID == UUID);
        return data?.Username ?? "Unreal";
    }
    internal Player? GetUserData(Guid UUID)
    {
        PD_Table? data = _connection?.Table<PD_Table>().FirstOrDefault(p => p.PlayerID == UUID);
        Player? player = new(data.Username) { PlayerID = data.PlayerID, Wins = data.Wins, Losses = data.Losses };
        return player ?? null;
    }
}

[Table("Player_Data")]
public class PD_Table : IPlayer
{
    [PrimaryKey]
    [Column("id")]
    public Guid PlayerID { get; set; }
    [Unique]
    [Column("username")]
    public string Username { get; init; }
    [Ignore]
    public List<GamePiece> GamePieces => null;
    [Column("wins")]
    public int Wins { get; set; }
    [Column("losses")]
    public int Losses { get; set; }
}
[Table("Login_Info")]
public class Auth_Table : AuthData
{
    [PrimaryKey]
    [Column("id")]
    public Guid PlayerID { get; set; }
    [Column("password")]
    public string Password { get; set; }
}
public interface AuthData
{
    public Guid PlayerID { get; set; }
    public string Password { get; set; }

}