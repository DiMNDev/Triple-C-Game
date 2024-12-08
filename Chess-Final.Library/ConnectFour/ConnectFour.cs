namespace Chess_Final.ConnectFour;
using Chess_Final.Generics;
using Chess_Final.Player;

public partial class ConnectFour : Game
{
    public ConnectFour() : base(GameType.ConnectFour)
    {
        Board = new(GameType.ConnectFour);
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