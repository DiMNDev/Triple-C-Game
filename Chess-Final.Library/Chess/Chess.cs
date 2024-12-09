namespace Chess_Final.Chess;

using Player;
using Generics;
using TC_DataManager;
using Chess_Final.Lobby;
using System.Security.Cryptography.X509Certificates;

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
        if (GameOver) GameOverCleanUp();
        Console.WriteLine($"{CurrentPlayer.Username} Has ended their turn.");
        base.NewTurn();
    }
    public void AppendMove(Player player, (ChessCoordinate X, int Y) movedTo)
    {
        CheckInCheck();
        MoveLog.Add((player, player.SelectedPiece, movedTo, player.Check));
        Console.WriteLine(MoveLog.Count());
    }
    public void CheckInCheck()
    {
        Player NextPlayer = CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;

        GamePiece King = NextPlayer.GamePieces.FirstOrDefault(gp => gp.Name == "King");

        (int X, int Y) KingPosition = ParsePosition(King.CurrentPosition);

        King.CalculateValidMoves(Board.GetPieceFromMatrix);

        List<(int X, int Y)> Threats = GenerateEnemyMoves(King.owner == Owner.Player ? Owner.Opponent : Owner.Player, UUID);

        NextPlayer.Check = Threats.Any(t => t == KingPosition);
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
            Console.WriteLine($"---King POS: ({move.X},{move.Y})---");
            for (int Y = 0; Y < temp.GetLength(0); Y++)
            {
                for (int X = 0; X < temp.GetLength(1); X++)
                {
                    if (temp[X, Y] != null && temp[X, Y].Name != "King")
                    {
                        Console.WriteLine($"Piece: {temp[X, Y].Name} POS: ({X},{Y})");
                        temp[X, Y].CalculateValidMoves(game.Board.GetPieceFromMatrix);

                        // Ignore Pawn Forward
                        if (temp[X, Y].Name == "Pawn")
                        {

                            (int pieceX, int _) = Chess.ParsePosition(temp[X, Y].CurrentPosition);
                            temp[X, Y].AllowedMovement.Where(mv => mv.X != pieceX);
                        }
                        else
                        {
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
                    (int X, int _) = Chess.ParsePosition(piece.CurrentPosition);
                    piece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                    foreach (var move in piece.AllowedMovement)
                    {
                        if (piece.Name != "Pawn")
                        {
                            EnemyMoves.Add(move);
                        }
                        else if (piece.Name == "Pawn" && move.X != X)
                        {
                            EnemyMoves.Add(move);
                        }
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
                    (int X, int _) = Chess.ParsePosition(piece.CurrentPosition);
                    piece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                    foreach (var move in piece.AllowedMovement)
                    {
                        if (piece.Name != "Pawn")
                        {
                            EnemyMoves.Add(move);
                        }
                        else if (piece.Name == "Pawn" && move.X != X)
                        {
                            EnemyMoves.Add(move);
                        }
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
                    ChessPiece newPiece = g.pieceType switch
                    {
                        "pawns" => new Pawn(Owner.Player, (piece.x, piece.y)) { Name = "Pawn", GameID = UUID },
                        "rooks" => new Rook(Owner.Player, (piece.x, piece.y)) { Name = "Rook", GameID = UUID },
                        "knights" => new Knight(Owner.Player, (piece.x, piece.y)) { Name = "Knight", GameID = UUID },
                        "bishops" => new Bishop(Owner.Player, (piece.x, piece.y)) { Name = "Bishop", GameID = UUID },
                        "queen" => new Queen(Owner.Player, (piece.x, piece.y)) { Name = "Queen", GameID = UUID },
                        "king" => new King(Owner.Player, (piece.x, piece.y)) { Name = "King", GameID = UUID },
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
                ChessPiece newPiece = g.pieceType switch
                {
                    "pawns" => new Pawn(Owner.Opponent, (piece.x, piece.y)) { Name = "Pawn", GameID = UUID },
                    "rooks" => new Rook(Owner.Opponent, (piece.x, piece.y)) { Name = "Rook", GameID = UUID },
                    "knights" => new Knight(Owner.Opponent, (piece.x, piece.y)) { Name = "Knight", GameID = UUID },
                    "bishops" => new Bishop(Owner.Opponent, (piece.x, piece.y)) { Name = "Bishop", GameID = UUID },
                    "queen" => new Queen(Owner.Opponent, (piece.x, piece.y)) { Name = "Queen", GameID = UUID },
                    "king" => new King(Owner.Opponent, (piece.x, piece.y)) { Name = "King", GameID = UUID },
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
public abstract class ChessPiece : GamePiece
{
    public abstract PieceType Type { get; set; }
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
