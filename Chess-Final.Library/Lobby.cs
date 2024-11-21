namespace Chess_Final.Lobby;
using Chess_Final.Chess;
using Chess_Final.Generics;
using Chess_Final.Player;

public class Lobby
{
    // UUID, Game
    public static Lobby Instance { get; } = new();
    public Dictionary<Guid, (bool active, bool open, Game game)> ChessGames = new();
    public Dictionary<Guid, (bool active, bool open, Game game)> CheckersGames = new();
    public Dictionary<Guid, (bool active, bool open, Game game)> ConnectFourGames = new();
    public Guid CreateGame(Player player, GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Chess:
                Chess chess = new Chess();
                chess.JoinGame(player);
                Instance.ChessGames.Add(chess.UUID, (false, true, chess));
                return chess.UUID;
            case GameType.Checkers: throw new NotImplementedException("Game logic does not exist");
            case GameType.ConnectFour: throw new NotImplementedException("Game logic does not exist");
            default:
                throw new InvalidGameTypeException("Sorry, fresh out..");
        }
    }
    // create new game
    // pull Guid -> add to respective dictionary
    public Game? GetGame(GameType gameType, Guid id)
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

    public List<(bool active, bool open, Game game)> FilterByOpen(GameType gameType)
    {
        return gameType switch
        {
            GameType.Chess => Instance.ChessGames.Where(dv => dv.Value.open == true).Select(g => g.Value).ToList(),
            GameType.Checkers => Instance.ChessGames.Where(dv => dv.Value.open == true).Select(g => g.Value).ToList(),
            GameType.ConnectFour => Instance.ChessGames.Where(dv => dv.Value.open == true).Select(g => g.Value).ToList(),
        };
    }

}