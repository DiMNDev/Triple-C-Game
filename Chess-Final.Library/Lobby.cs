namespace Chess_Final.Lobby;
using Chess_Final.Chess;
using Chess_Final.Generics;
using Microsoft.VisualBasic;

public static class LobbyManager
{
    // UUID, Game
    // public static Lobby Instance { get; } = new();
    public static Dictionary<Guid, Game> ChessGames = new();
    public static Dictionary<Guid, Game> CheckersGames = new();
    public static Dictionary<Guid, Game> ConnectFourGames = new();
    public static event Action LobbyChanged;

    public static Guid CreateGame(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Chess:
                Chess chess = new Chess();
                // chess.JoinGame(player);
                ChessGames.Add(chess.UUID, chess);
                LobbyChanged?.Invoke();
                return chess.UUID;
            case GameType.Checkers: throw new NotImplementedException("Game logic does not exist");
            case GameType.ConnectFour: throw new NotImplementedException("Game logic does not exist");
            default:
                throw new InvalidGameTypeException("Sorry, fresh out..");
        }
    }
    // create new game
    // pull Guid -> add to respective dictionary

    public static Game? GetGame(GameType gameType, Guid id)
    {
        // get game by type, search by uuid?
        return gameType switch
        {
            GameType.Chess => ChessGames.FirstOrDefault(UUID => UUID.Key == id).Value,
            GameType.Checkers => CheckersGames.FirstOrDefault(UUID => UUID.Key == id).Value,
            GameType.ConnectFour => ConnectFourGames.FirstOrDefault(UUID => UUID.Key == id).Value,
            _ => throw new GameNotFoundException("Game not found. Does it Exist?")
        };
    }

    public static Dictionary<Guid, Game> FilterByOpen(GameType gameType)
    {
        return gameType switch
        {
            GameType.Chess => ChessGames.Where(g => g.Value.Open == true).ToDictionary(k => k.Key, v => v.Value),
            GameType.Checkers => CheckersGames.Where(g => g.Value.Open == true).ToDictionary(k => k.Key, v => v.Value),
            GameType.ConnectFour => ConnectFourGames.Where(g => g.Value.Open == true).ToDictionary(k => k.Key, v => v.Value),
        };
    }

    public static Dictionary<Guid, Game> GetCurrentLobby(GameType gameType)
    {
        return gameType switch
        {
            GameType.Chess => ChessGames,
            GameType.Checkers => CheckersGames,
            GameType.ConnectFour => ConnectFourGames,
            _ => throw new GameNotFoundException("Game not found. Does it Exist?")
        };

    }
    public static GameType ConvertStringToGameType(string gameType)
    {
        GameType lobbyType = gameType switch
        {
            "Chess" => GameType.Chess,
            "Checkers" => GameType.Checkers,
            "ConnectFour" => GameType.ConnectFour,
        };
        return lobbyType;
    }
}