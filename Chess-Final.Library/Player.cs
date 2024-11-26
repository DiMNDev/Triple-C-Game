namespace Chess_Final.Player;
using Generics;

public class Player : IPlayer
{
    public List<GamePiece> GamePieces { get; set; } = new();
    public string Username { get; init; }
    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
    public Guid PlayerID { get; set; }
    public GamePiece SelectedPiece { get; set; } = null;
    public event Action GameHasChanged;

    public Player(string name, Guid? guid = null)
    {
        Username = name;
        PlayerID = guid ?? Guid.NewGuid();
    }
    public void SetSelected(int X, int Y)
    {
        Console.WriteLine($"Clicked: ({X},{Y})");
        SelectedPiece = GamePieces.Where(p => p.CurrentPosition == (((ChessCoordinate)X).ToString(), Y)).FirstOrDefault();
        GameHasChanged?.Invoke();
    }
}
