namespace Chess_Final.Generics;

using Player;

public interface IPlayer
{
    public Guid PlayerID { get; set; }
    public string Username { get; init; }
    public List<GamePiece> GamePieces { get; }
    public int Wins { get; set; }
    public int Losses { get; set; }

}
public abstract class GamePiece
{
    public string Name { get; set; }
    public abstract Owner owner { get; init; }
    public List<(int X, int Y)> AllowedMovement { get; set; } = new();
    public bool CanMove { get; set; }
    // On set run CalculateValidMoves instead of solely constructor?
    public abstract (string X, int Y) CurrentPosition { get; set; }
    public abstract void CalculateValidMoves();
    public abstract void MovePiece(Game game, Player player);

}

public class GameBoard
{
    public GameType Type { get; init; }
    public (int X, int Y)? BoardSize { get; private set; }
    public GamePiece[,]? Matrix { get; private set; }

    public GameBoard(GameType gameType)
    {
        (int X, int Y) Size = gameType switch
        {
            GameType.Chess => (8, 8),
            GameType.Checkers => (8, 8),
            GameType.ConnectFour => (6, 7),
            _ => throw new GameNotFoundException()
        };
        Type = gameType;
        BoardSize = Size;
        Matrix = new GamePiece[Size.X, Size.Y];
    }
}
public abstract class Game
{
    public Guid UUID { get; set; } = Guid.NewGuid();
    public GameType Type { get; init; }
    public GameBoard? Board { get; set; } = null;
    public Player? CurrentPlayer { get; set; } = null;
    public Player? PlayerOne { get; set; } = null;
    public Player? PlayerTwo { get; set; } = null;
    public bool Active = false;
    public bool Open = true;
    public List<Player> Spectators { get; set; } = new();
    public Player? Winner { get; set; } = null;
    public bool GameOver { get; set; } = false;
    public static event Action GameChanged;
    protected Game(GameType game)
    {
        Type = game;
        Board = new GameBoard(Type);
    }
    private void IsGameReady()
    {
        if (PlayerOne != null && PlayerTwo != null)
        {
            Open = false;
            CurrentPlayer = PlayerOne;
            GameChanged?.Invoke();
        }
    }
    public abstract void LayoutGamePieces(Player player);
    public void JoinGame(Player player)
    {
        if (PlayerOne == null)
        {
            PlayerOne = player;
            LayoutGamePieces(player);
            IsGameReady();
            GameChanged?.Invoke();
        }
        else if (PlayerTwo == null)
        {
            PlayerTwo = player;
            LayoutGamePieces(player);
            IsGameReady();
            GameChanged?.Invoke();
        }
        else
        {
            Spectators.Add(player);
        }
    }
    public abstract GamePiece? PlaceGamePiece(int x, int y);
    public abstract void PlaceInMatrix();
    protected void UpdateGame()
    {
        GameChanged?.Invoke();
    }
}

public enum GameType
{
    Chess,
    Checkers,
    ConnectFour
}

public enum ChessCoordinate
{
    A, B, C, D, E, F, G, H
}
public enum Owner
{
    Player,
    Opponent
}

public static class FilePaths
{
    public static string Tests { get; } = "../../../../Chess-Final.Library/ChessLayout.json";
    public static string Blazor { get; } = "../Chess-Final.Library/ChessLayout.json";
    public static string Console { get; } = "";
}