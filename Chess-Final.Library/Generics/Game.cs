namespace Chess_Final.Generics;

using Chess_Final.DB_Manager;
using Chess_Final.Lobby;
using Player;

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

        switch (game)
        {
            case GameType.Chess:
                LobbyManager.ChessGames.Add(UUID, this);
                break;
            case GameType.Checkers:
                LobbyManager.CheckersGames.Add(UUID, this);
                break;
            case GameType.ConnectFour:
                LobbyManager.ConnectFourGames.Add(UUID, this);
                break;
        }
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
