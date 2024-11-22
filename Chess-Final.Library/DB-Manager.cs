namespace Chess_Final.DB_Manager;
using Chess_Final.Generics;
using SQLite;
using Player;


public class DB_Connect
{
    SQLiteConnection? _connection;

    public string ConnectionString { get; private set; } = "TC_DB.sqlite3";

    public void IntializeConnection()
    {
        SQLiteConnectionString options = new SQLiteConnectionString(ConnectionString, false);
        _connection = new SQLiteConnection(options);
        _connection.CreateTable<PD_Table>();
    }

    public bool InsertRecord(Player player)
    {
        // create a new record
        var result = _connection.Insert(player);
        return result;
    }
    public bool VerifyAccount()
    {
        // check username and password match in database
    }
    public Player LoadUserData()
    {
        // if guid matches create new player and add to players online
        return new Player("newPlayer");

    }

}

[Table("Player_Data")]
public class PD_Table : IPlayer
{
    [PrimaryKey]
    [Column("id")]
    public Guid PlayerID { get; set; }
    [Column("name")]
    public string Name { get; init; }
    [Ignore]
    public List<GamePiece> GamePieces => null;
    [Column("wins")]
    public int Wins { get; set; }
    [Column("losses")]
    public int Losses { get; set; }
}

[Table("Login_Info")]
public class Auth_Table
{
    [PrimaryKey]
    [Column("username")]
    public string Username { get; init; }
    [Column("password")]
    public string Password { get; set; }
    [Column("id")]
    public Guid PlayerID { get; set; }


}