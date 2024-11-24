namespace Chess_Final.Lobby;
using Chess_Final.Chess;
using Chess_Final.Generics;

public static class LobbyManager
{
    // UUID, Game
    // public static Lobby Instance { get; } = new();
    public static Dictionary<Guid, (bool active, bool open, Game game)> ChessGames = new();
    public static Dictionary<Guid, (bool active, bool open, Game game)> CheckersGames = new();
    public static Dictionary<Guid, (bool active, bool open, Game game)> ConnectFourGames = new();
    public static Guid CreateGame(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Chess:
                Chess chess = new Chess();
                // chess.JoinGame(player);
                ChessGames.Add(chess.UUID, (false, true, chess));
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
            GameType.Chess => ChessGames.Select(g => g.Value.game).FirstOrDefault(i => i.UUID == id),
            GameType.Checkers => CheckersGames.Select(g => g.Value.game).FirstOrDefault(i => i.UUID == id),
            GameType.ConnectFour => ConnectFourGames.Select(g => g.Value.game).FirstOrDefault(i => i.UUID == id),
            _ => throw new GameNotFoundException("Game not found. Does it Exist?")
        };
    }

    public static List<(bool active, bool open, Game game)> FilterByOpen(GameType gameType)
    {
        return gameType switch
        {
            GameType.Chess => ChessGames.Where(dv => dv.Value.open == true).Select(g => g.Value).ToList(),
            GameType.Checkers => CheckersGames.Where(dv => dv.Value.open == true).Select(g => g.Value).ToList(),
            GameType.ConnectFour => ConnectFourGames.Where(dv => dv.Value.open == true).Select(g => g.Value).ToList(),
        };
    }

    public static Dictionary<Guid, (bool active, bool open, Game game)> GetCurrentLobby(GameType gameType)
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