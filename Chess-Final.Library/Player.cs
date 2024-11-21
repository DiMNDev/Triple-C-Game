namespace Chess_Final.Player;
using Generics;

public class Player : IPlayer
{
    public List<GamePiece> GamePieces { get; set; } = new();
    public string Name { get; init; }
    public int Wins { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Losses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Guid PlayerID { get; set; }

    public Player(string name, Guid? guid = null)
    {
        Name = name;
        PlayerID = guid ?? Guid.NewGuid();
    }
}
