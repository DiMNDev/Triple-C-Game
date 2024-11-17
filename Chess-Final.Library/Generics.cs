namespace Chess_Final.Generics;

using Player;

public interface IPlayer
{
    public string Name { get; init; }
    public List<GamePiece>? GamePieces { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }

}
public abstract class GamePiece
{
    public string Name { get; set; }
    public (int X, int Y) AllowedMovement { get; set; }
    public bool CanMove { get; set; }
    public (int X, int Y) CurrentPosition { get; set; }

}

public class GameBoard
{
    public GameType Type { get; init; }
    public (int X, int Y)? BoardSize { get; private set; }
    public GamePiece[,]? Matrix { get; private set; } = null;

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
    public GameType Type { get; init; }
    public GameBoard? Board { get; set; } = null;
    public Player? PlayerOne { get; set; } = null;
    public Player? PlayerTwo { get; set; } = null;
    public Player? Winner { get; set; } = null;
    public bool GameOver { get; set; } = false;
    protected Game(GameType game)
    {
        Type = game;
        Board = new GameBoard(Type);

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