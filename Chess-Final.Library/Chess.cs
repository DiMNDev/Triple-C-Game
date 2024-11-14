namespace Chess_Final.Chess;
using Generics;
using Player;

public class Pawn : GamePiece
{
    public string Name { get; set; }
    public (int X, int Y) AllowedMovement { get; set; }
    public bool CanMove { get; set; }
    public (int X, int Y) CurrentPosition { get; set; }
}

public class Chess : Game
{
    public Chess() : base(GameType.Chess)
    {
    }

    public string Name { get; private set; } = "Chess";
    public GameBoard Board { get; set; }
    public void LayoutGamePieces() { }
}