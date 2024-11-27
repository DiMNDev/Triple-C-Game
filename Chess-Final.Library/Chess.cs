namespace Chess_Final.Chess;

using Player;
using Generics;
using TC_DataManager;

public class ChessPieces
{

    public class Pawn : GamePiece
    {
        public PieceType Type { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public Pawn(Owner owner, (string X, int Y) currentPosition)
        {
            this.owner = owner;
            CurrentPosition = (currentPosition.X, currentPosition.Y);
        }
        public override void CalculateValidMoves(Func<int, int, GamePiece?> FindOpponent)
        {
            Console.WriteLine($"Calculating Valid move for {owner} : ({CurrentPosition.X}, {CurrentPosition.Y})");
            // Parse CurrentPosition
            Enum.TryParse<ChessCoordinate>(this.CurrentPosition.X, out ChessCoordinate ParsedX);
            int CurrentX = (int)ParsedX;
            int CurrentY = this.CurrentPosition.Y;
            // If Piece is PlayerOne Piece
            if (this.owner == Owner.Player)
            {
                // On First move can move 2 spacese
                if (FirstMove)
                {
                    // Set AllowedMove to move 2 spaces
                    AllowedMovement.Add((CurrentX, CurrentY - 2));
                    AllowedMovement.Add(((int)ParsedX, CurrentPosition.Y - 1));
                }
                else
                {
                    // Regular move -- Can move 1 space forward                    
                    AllowedMovement.Add(((int)ParsedX, CurrentPosition.Y - 1));
                }
                // If FindOpponent method is not null check if can attack (x+-1,y-1)
                if (FindOpponent != null)
                {
                    // Check if piece to right is opponent
                    var pieceToRight = FindOpponent(CurrentX + 1, CurrentY - 1);
                    // Check if piece to left is opponent
                    var pieceToLeft = FindOpponent(CurrentX - 1, CurrentY - 1);
                    Console.WriteLine(pieceToLeft);
                    Console.WriteLine(pieceToRight);
                    // Set allowed movement if opponent piece to right exists
                    if (pieceToRight != null)
                    {
                        AllowedMovement.Add((CurrentX + 1, CurrentY - 1));
                    }
                    // Set allowed movement if opponent piece to left exists
                    if (pieceToLeft != null)
                    {
                        AllowedMovement.Add((CurrentX - 1, CurrentY - 1));
                    }
                }
            }
            else if (this.owner == Owner.Opponent)
            {

                // On First move can move 2 spacese
                if (FirstMove)
                {
                    // Set AllowedMove to move 2 spaces
                    AllowedMovement.Add((CurrentX, CurrentY + 2));
                    AllowedMovement.Add(((int)ParsedX, CurrentPosition.Y + 1));
                }
                else
                {
                    // Regular move -- Can move 1 space forward                    
                    AllowedMovement.Add(((int)ParsedX, CurrentPosition.Y + 1));
                }
                // If FindOpponent method is not null check if can attack (x+-1,y-1)
                if (FindOpponent != null)
                {
                    // Check if piece to right is opponent
                    var pieceToRight = FindOpponent(CurrentX + 1, CurrentY + 1);
                    // Check if piece to left is opponent
                    var pieceToLeft = FindOpponent(CurrentX - 1, CurrentY + 1);


                    Console.WriteLine(pieceToLeft);
                    Console.WriteLine(pieceToRight);

                    // Set allowed movement if opponent piece to right exists
                    if (pieceToRight != null)
                    {
                        AllowedMovement.Add((CurrentX + 1, CurrentY + 1));
                    }
                    // Set allowed movement if opponent piece to left exists
                    if (pieceToLeft != null)
                    {
                        AllowedMovement.Add((CurrentX - 1, CurrentY + 1));
                    }
                }
            }
            Console.WriteLine($"Allowed: {string.Join(",", AllowedMovement)}");
        }

    }
    public class Rook : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
        {

        }
    }
    public class Knight : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
        {

        }
    }
    public class Bishop : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
        {

        }
    }
    public class Queen : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
        {

        }
    }
    public class King : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
        {

        }
    }
}

public class Chess : Game
{
    public static Chess Instance { get; set; } = new Chess();
    public Chess() : base(GameType.Chess)
    {
        Board = new(GameType.Chess);
    }
    public string Name { get; private set; } = "Chess";
    public override Player? CurrentPlayer { get; set; }

    public override void LayoutGamePieces(Player player)
    {
        // Check CWD for use with different projects
        string CWD = Directory.GetCurrentDirectory();
        Console.WriteLine(CWD);

        IEnumerable<PlayerData> data = DataManager.LoadFile<IEnumerable<PlayerData>>(FilePaths.Tests);
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
                        "pawns" => new ChessPieces.Pawn(Owner.Player, (piece.x, piece.y)) { Name = "Pawn", Type = PieceType.pawn },
                        "rooks" => new ChessPieces.Rook { Name = "Rook", Type = PieceType.rook, CurrentPosition = (piece.x, piece.y) },
                        "knights" => new ChessPieces.Knight { Name = "Knight", Type = PieceType.knight, CurrentPosition = (piece.x, piece.y) },
                        "bishops" => new ChessPieces.Bishop { Name = "Bishop", Type = PieceType.bishop, CurrentPosition = (piece.x, piece.y) },
                        "queen" => new ChessPieces.Queen { Name = "Queen", Type = PieceType.queen, CurrentPosition = (piece.x, piece.y) },
                        "king" => new ChessPieces.King { Name = "King", Type = PieceType.king, CurrentPosition = (piece.x, piece.y) },
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
                    "pawns" => new ChessPieces.Pawn(Owner.Opponent, (piece.x, piece.y)) { Name = "Pawn", Type = PieceType.pawn, CurrentPosition = (piece.x, piece.y) },
                    "rooks" => new ChessPieces.Rook { Name = "Rook", Type = PieceType.rook, CurrentPosition = (piece.x, piece.y) },
                    "knights" => new ChessPieces.Knight { Name = "Knight", Type = PieceType.knight, CurrentPosition = (piece.x, piece.y) },
                    "bishops" => new ChessPieces.Bishop { Name = "Bishop", Type = PieceType.bishop, CurrentPosition = (piece.x, piece.y) },
                    "queen" => new ChessPieces.Queen { Name = "Queen", Type = PieceType.queen, CurrentPosition = (piece.x, piece.y) },
                    "king" => new ChessPieces.King { Name = "King", Type = PieceType.king, CurrentPosition = (piece.x, piece.y) },
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
    public override GamePiece? PlaceGamePiece(int x, int y)
    {
        var piece = PlayerOne.GamePieces.Where(p => p.CurrentPosition == (((ChessCoordinate)x).ToString(), y)).FirstOrDefault();
        if (piece != null)
        {
            return piece;
        }
        else
        {
            return null;
        }
    }

    public bool MoveGamePiece(Game game, Player player, GamePiece piece, (ChessCoordinate X, int Y) moveTo)
    {
        Enum.TryParse<ChessCoordinate>(piece.CurrentPosition.X, out ChessCoordinate ParsedX);
        int movement = (int)moveTo.X + (int)ParsedX;
        if (Board.Matrix[movement, moveTo.Y] == null)
        {
            piece.CurrentPosition = (((ChessCoordinate)movement).ToString(), moveTo.Y);
            return true;
        }
        return false;
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
