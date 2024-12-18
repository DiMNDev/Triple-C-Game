namespace Chess_Final.Generics;

//REQ#2.2.1
public interface IPlayer
{
    public Guid PlayerID { get; set; }
    public string Username { get; init; }
    public List<GamePiece> GamePieces { get; }
    public int Wins { get; set; }
    public int Losses { get; set; }

}
