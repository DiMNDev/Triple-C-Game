namespace Chess_Final.Lobby;
using Chess_Final.Chess;
using Chess_Final.Generics;


public static class Lobby
{
    // UUID, Game
    public static Dictionary<Guid, (bool active, bool open, Game game)> ChessGames = new();
    public static Dictionary<Guid, (bool active, bool open, Game game)> CheckersGames = new();
    public static Dictionary<Guid, (bool active, bool open, Game game)> ConnectFourGames = new();

    public static void CreateGame(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.Chess:
                Chess chess = new();
                ChessGames.Add(chess.UUID, (false, true, chess));
                break;
        }
        // create new game
        // pull Guid -> add to respective dictionary
    }
    public static Game GetGame(GameType type)
    {
        // get game by type, search by uuid?
        return Chess.Chess.CurrentGame;
    }

    public static IEnumerable<Dictionary<Guid, (bool active, bool open, Game game)>> FilterActiveGames()
    {
        return ChessList;
    }
    public static IEnumerable<Dictionary<Guid, (bool active, bool open, Game game)>> FilterOpenGames()
    {
        return ChessList;
    }
    public static IEnumerable<Dictionary<Guid, (bool active, bool open, Game game)>> ChessList;
    public static IEnumerable<Dictionary<Guid, (bool active, bool open, Game game)>> CheckersList;
    public static IEnumerable<Dictionary<Guid, (bool active, bool open, Game game)>> ConnectFourList;
}