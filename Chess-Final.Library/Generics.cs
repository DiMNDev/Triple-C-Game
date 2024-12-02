namespace Chess_Final.Generics;

using Chess_Final.DB_Manager;
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
    public Guid GameID { get; set; }
    public bool FirstMove { get; set; } = true;
    public List<(int X, int Y)> AllowedMovement { get; set; } = new();
    public bool CanMove { get; set; }
    // On set run CalculateValidMoves instead of solely constructor?
    public abstract (string X, int Y) CurrentPosition { get; set; }
    public abstract void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent);

}
public class GameBoard
{
    public GameType Type { get; init; }
    public (int X, int Y)? BoardSize { get; private set; }
    public GamePiece[,]? Matrix { get; private set; }
    public GamePiece[,]? TempMatrix { get => HardCopy(); set => _ = value; }

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
    private GamePiece[,] HardCopy()
    {
        GamePiece[,] temp = new GamePiece[8, 8];
        for (int Y = 0; Y < Matrix.GetLength(0); Y++)
        {
            for (int X = 0; X < Matrix.GetLength(1); X++)
            {
                temp[X, Y] = Matrix[X, Y];
            }
        }
        return temp;
    }

    public GamePiece? GetPieceFromMatrix(int x, int y)
    {
        if (x < Matrix.GetLength(0) && x >= 0 && y < Matrix.GetLength(1) && y >= 0)
        {
            return Matrix[x, y];
        }
        else { return null; }
    }

    public void resetMatrix()
    {
        for (int y = 0; y < Matrix.GetLength(0); y++)
        {
            for (int x = 0; x < Matrix.GetLength(1); x++)
            {
                Matrix[x, y] = null;
            }
        }
    }
}
public abstract class Game
{
    public Guid UUID { get; set; } = Guid.NewGuid();
    public GameType Type { get; init; }
    public GameBoard? Board { get; set; } = null;
    public abstract Player? CurrentPlayer { get; set; }
    public Player? PlayerOne { get; set; } = null;
    public Player? PlayerTwo { get; set; } = null;
    public bool Active = false;
    public bool Open = true;
    public List<Player> Spectators { get; set; } = [];
    public Player? Winner { get; set; } = null;
    public bool GameOver { get; set; } = false;
    public static event Action GameChanged;
    public virtual void NewTurn()
    {
        _ = CurrentPlayer == PlayerOne ? CurrentPlayer = PlayerTwo : CurrentPlayer = PlayerOne;
        CurrentPlayer!.TurnOver += NewTurn;
        GameChanged?.Invoke();
    }
    public virtual void GameOverCleanUp()
    {
        GameOver = true;
        Winner = CurrentPlayer;
        Winner.Wins += 1;
        Player Loser = Winner == PlayerOne ? PlayerTwo : PlayerOne;
        Loser.Losses += 1;
        DB_Connect dB_Connect = new();
        dB_Connect.UpdateRecord(Winner);
        dB_Connect.UpdateRecord(Loser);
        GameChanged?.Invoke(); // trigger to update lobbies?        
    }
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
            CurrentPlayer.TurnOver += NewTurn;
        }
    }
    public abstract void LayoutGamePieces(Player player);
    public void JoinGame(Player player, JoinAs joinAs)
    {
        if (joinAs == JoinAs.Player)
        {

            if (PlayerOne == null)
            {
                PlayerOne = player;
                LayoutGamePieces(player);
                IsGameReady();

            }
            else if (PlayerTwo == null)
            {
                PlayerTwo = player;
                LayoutGamePieces(player);
                IsGameReady();

            }
        }
        else if (joinAs == JoinAs.Spectator)
        {
            Spectators.Add(player);

        }
    }
    public abstract void PlaceInMatrix();
    protected void UpdateGame()
    {
        GameChanged?.Invoke();
    }

    internal GamePiece[,] HardCopy()
    {
        throw new NotImplementedException();
    }
}
public enum JoinAs
{
    Player,
    Spectator
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