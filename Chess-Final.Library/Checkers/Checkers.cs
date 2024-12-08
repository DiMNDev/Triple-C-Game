namespace Chess_Final.Checkers;
using Chess_Final.Generics;
using Chess_Final.Player;

public class Checkers : Game
{

    public Checkers() : base(GameType.Checkers)
    {
        Board = new(GameType.Checkers);
        throw new GameNotImplementedException();
    }

    public override Player? CurrentPlayer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void LayoutGamePieces(Player player)
    {
        throw new NotImplementedException();
    }

    public override void PlaceInMatrix()
    {
        throw new NotImplementedException();
    }
}