namespace Chess_Final.Generics;

public abstract class GamePiece
{
    public string Name { get; set; }
    public abstract Owner owner { get; init; }
    public bool RemovedFromPlay { get; set; } = false;
    public Guid GameID { get; set; }
    public bool FirstMove { get; set; } = true;
    public List<(int X, int Y)> AllowedMovement { get; set; } = new();
    public bool CanMove { get; set; }
    // On set run CalculateValidMoves instead of solely constructor?
    public abstract (string X, int Y) CurrentPosition { get; set; }
    public abstract void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent);

}
