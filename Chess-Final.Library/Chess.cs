namespace Chess_Final.Chess;

using Player;
using Generics;
using TC_DataManager;
using Chess_Final.Lobby;

public class Chess : Game
{
    public static Chess Instance { get; set; } = new Chess();
    public Chess() : base(GameType.Chess)
    {
        Board = new(GameType.Chess);
    }
    public string Name { get; private set; } = "Chess";
    public override Player? CurrentPlayer { get; set; }
    public override void NewTurn()
    {
        CheckInCheck();
        if (GameOver) GameOverCleanUp();
        Console.WriteLine($"{CurrentPlayer.Username} Has ended their turn.");
        base.NewTurn();
    }
    public void CheckInCheck()
    {
        Player NextPlayer = CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;

        Console.WriteLine($"CurrentPlayer: {CurrentPlayer.Username}");
        Console.WriteLine($"NextPlayer: {NextPlayer.Username}");

        GamePiece King = NextPlayer.GamePieces.FirstOrDefault(gp => gp.Name == "King");

        (int X, int Y) KingPosition = ParsePosition(King.CurrentPosition);

        King.CalculateValidMoves(Board.GetPieceFromMatrix);

        List<(int X, int Y)> Threats = GenerateEnemyMoves(King.owner == Owner.Player ? Owner.Opponent : Owner.Player, UUID);

        NextPlayer.Check = Threats.Any(t => t == KingPosition);
        // if(NextPlayer.Check) GameOver = !Threats.Any(t=> King.AllowedMovement.Any(mv => t == mv));
        if (NextPlayer.Check && King.AllowedMovement.Count == 0) GameOver = true;

        Console.WriteLine($"{NextPlayer.Username} in check: {NextPlayer.Check}");
    }

    public static List<(int X, int Y)>? ValidateSafeMovesForKing(Owner owner, List<(int X, int Y)> possibleMoves, Guid gameID)
    {
        Game game = LobbyManager.GetGame(GameType.Chess, gameID);


        GamePiece King = owner switch
        {
            Owner.Player => game.PlayerOne.GamePieces.Where(p => p.Name == "King").FirstOrDefault(),
            Owner.Opponent => game.PlayerTwo.GamePieces.Where(p => p.Name == "King").FirstOrDefault(),

        };
        List<(int X, int Y)> returnList = new();
        foreach (var move in possibleMoves)
        {
            GamePiece[,] temp = game.Board.TempMatrix;
            temp[move.X, move.Y] = King;
            for (int Y = 0; Y < temp.GetLength(0); Y++)
            {
                for (int X = 0; X < temp.GetLength(1); X++)
                {
                    if (temp[X, Y] != null)
                    {
                        if (temp[X, Y].Name != "King")
                        {
                            temp[X, Y].CalculateValidMoves(game.Board.GetPieceFromMatrix);
                            returnList.AddRange(temp[X, Y].AllowedMovement.Where(mv => returnList.Contains(mv)));
                        }
                    }
                }
            }
        }
        return returnList;
    }
    public static List<(int X, int Y)> GenerateEnemyMoves(Owner owner, Guid gameID)
    {
        List<(int X, int Y)> EnemyMoves = new();
        Game game = LobbyManager.GetGame(GameType.Chess, gameID);
        if (owner == Owner.Player)
        {
            foreach (var piece in game.PlayerOne.GamePieces)
            {
                if (piece.Name != "King")
                {
                    piece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                    foreach (var move in piece.AllowedMovement)
                    {
                        EnemyMoves.Add(move);
                    }
                }
            }
        }
        if (owner == Owner.Opponent)
        {
            foreach (var piece in game.PlayerTwo.GamePieces)
            {
                if (piece.Name != "King")
                {
                    piece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                    foreach (var move in piece.AllowedMovement)
                    {
                        EnemyMoves.Add(move);
                    }
                }
            }
        }
        return EnemyMoves;

    }
    public static List<(int X, int Y)> GetPlayerPiecePositions(Owner owner, Guid gameID, GamePiece excluded)
    {
        List<(int X, int Y)> PiecePositions = new();
        Game game = LobbyManager.GetGame(GameType.Chess, gameID);
        if (owner == Owner.Player)
        {
            foreach (var piece in game.PlayerOne.GamePieces)
            {
                if (piece != excluded)
                {
                    (int, int) ParsedPosition = Chess.ParsePosition(piece.CurrentPosition);
                    PiecePositions.Add(ParsedPosition);
                }
            }
        }
        if (owner == Owner.Opponent)
        {
            foreach (var piece in game.PlayerTwo.GamePieces)
            {

                if (piece != excluded)
                {
                    (int, int) ParsedPosition = Chess.ParsePosition(piece.CurrentPosition);
                    PiecePositions.Add(ParsedPosition);
                }

            }
        }
        return PiecePositions;

    }
    public static (int X, int Y) ParsePosition((string X, int Y) position)
    {
        Enum.TryParse<ChessCoordinate>(position.X, out ChessCoordinate ParsedX);
        int CurrentX = (int)ParsedX;
        return (CurrentX, position.Y);
    }
    public override void LayoutGamePieces(Player player)
    {
        // Check CWD for use with different projects
        string CWD = Directory.GetCurrentDirectory();
        Console.WriteLine(CWD);

        IEnumerable<PlayerData> data = DataManager.LoadFile<IEnumerable<PlayerData>>(FilePaths.Blazor);
        if (data != null)
            if (player == PlayerOne)
            {
                {
                    var playerPieces = data.Where(p => p.playerType == "player")
                    .Select(d => d.data)
                    .SelectMany(p => p)
                    .SelectMany(g => g.pieces.Select(piece =>
                {
                    GamePiece newPiece = g.pieceType switch
                    {
                        "pawns" => new Pawn(Owner.Player, (piece.x, piece.y)) { Name = "Pawn", Type = PieceType.pawn, GameID = UUID },
                        "rooks" => new Rook(Owner.Player, (piece.x, piece.y)) { Name = "Rook", Type = PieceType.rook, GameID = UUID },
                        "knights" => new Knight(Owner.Player, (piece.x, piece.y)) { Name = "Knight", Type = PieceType.knight, GameID = UUID },
                        "bishops" => new Bishop(Owner.Player, (piece.x, piece.y)) { Name = "Bishop", Type = PieceType.bishop, GameID = UUID },
                        "queen" => new Queen(Owner.Player, (piece.x, piece.y)) { Name = "Queen", Type = PieceType.queen, GameID = UUID },
                        "king" => new King(Owner.Player, (piece.x, piece.y)) { Name = "King", Type = PieceType.king, GameID = UUID },
                    };
                    PlayerOne.GamePieces.Add(newPiece);
                    // Janky
                    return newPiece;
                }
                    )
                     ).ToList();
                }
            }
        if (player == PlayerTwo)
        {
            {
                var opponentPieces = data.Where(p => p.playerType == "opponent")
                .Select(d => d.data)
                .SelectMany(p => p)
                .SelectMany(g => g.pieces.Select(piece =>
            {
                GamePiece newPiece = g.pieceType switch
                {
                    "pawns" => new Pawn(Owner.Opponent, (piece.x, piece.y)) { Name = "Pawn", Type = PieceType.pawn, GameID = UUID },
                    "rooks" => new Rook(Owner.Opponent, (piece.x, piece.y)) { Name = "Rook", Type = PieceType.rook, GameID = UUID },
                    "knights" => new Knight(Owner.Opponent, (piece.x, piece.y)) { Name = "Knight", Type = PieceType.knight, GameID = UUID },
                    "bishops" => new Bishop(Owner.Opponent, (piece.x, piece.y)) { Name = "Bishop", Type = PieceType.bishop, GameID = UUID },
                    "queen" => new Queen(Owner.Opponent, (piece.x, piece.y)) { Name = "Queen", Type = PieceType.queen, GameID = UUID },
                    "king" => new King(Owner.Opponent, (piece.x, piece.y)) { Name = "King", Type = PieceType.king, GameID = UUID },
                };
                PlayerTwo.GamePieces.Add(newPiece);
                // Janky
                return newPiece;
            }
                )
                 ).ToList();
            }
        }
    }

    // Should run when both players are in game
    public override void PlaceInMatrix()
    {
        if (PlayerOne != null)
        {
            foreach (var piece in PlayerOne.GamePieces)
            {
                Enum.TryParse<ChessCoordinate>(piece.CurrentPosition.X, out ChessCoordinate X);
                Board.Matrix[(int)X, piece.CurrentPosition.Y] = piece;

            }
        }
        if (PlayerTwo != null)
        {
            foreach (var piece in PlayerTwo.GamePieces)
            {
                Enum.TryParse<ChessCoordinate>(piece.CurrentPosition.X, out ChessCoordinate X);
                Board.Matrix[(int)X, piece.CurrentPosition.Y] = piece;
            }
        }
        UpdateGame();
    }
}

public enum PieceType
{
    pawn,
    rook,
    knight,
    bishop,
    queen,
    king
}
