namespace Chess_Final.Player;
using Generics;

public class Player : IPlayer
{
    public List<GamePiece>? GamePieces { get; set; } = null;
    public string Name { get; init; }
    public int Wins { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int Losses { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Player(string name)
    {
        Name = name;
    }
}
