namespace Chess_Final.Chess;

using Player;
using Generics;
using TC_DataManager;

public class ChessPieces
{

    public class Pawn : GamePiece
    {
        public PieceType Type { get; set; }
        public bool FirstMove { get; private set; } = true;
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }

        public Pawn(Owner owner, (string X, int Y) currentPosition)
        {
            this.owner = owner;
            CurrentPosition = (currentPosition.X, currentPosition.Y);
            CalculateValidMoves();

        }
        public override void CalculateValidMoves()
        {
            Enum.TryParse<ChessCoordinate>(this.CurrentPosition.X, out ChessCoordinate ParsedX);
            int CurrentX = (int)ParsedX;
            int CurrentY = this.CurrentPosition.Y;

            if (this.owner == Owner.Player)
            {
                if (FirstMove)
                {
                    var ValidMove01 = (CurrentX, CurrentY - 2);
                    AllowedMovement.Add(ValidMove01);
                }
                else
                {
                    var ValidMove02 = ((int)ParsedX, CurrentPosition.Y - 1);
                    AllowedMovement.Add(ValidMove02);
                }

            }
            else if (this.owner == Owner.Opponent)
            {

                if (FirstMove)
                {
                    var ValidMove01 = (CurrentX, CurrentY + 2);
                    AllowedMovement.Add(ValidMove01);
                }
                else
                {
                    var ValidMove02 = (CurrentX, CurrentY + 1);
                    AllowedMovement.Add(ValidMove02);
                }
            }

        }

        public override void MovePiece(Game game, Player player)
        {
            Enum.TryParse<ChessCoordinate>(CurrentPosition.X, out ChessCoordinate result);
            if (player == game.PlayerOne)
            {

                if (FirstMove)
                {
                    AllowedMovement.Add(((int)result, CurrentPosition.Y - 2));
                }
                else
                {
                    AllowedMovement.Add(((int)result, CurrentPosition.Y - 1));
                    AllowedMovement.Remove(((int)result, CurrentPosition.Y - 2));
                }
            }
            else if (player == game.PlayerTwo)
            {

                if (FirstMove)
                {
                    AllowedMovement.Add(((int)result, CurrentPosition.Y + 2));
                }
                else
                {
                    AllowedMovement.Add(((int)result, CurrentPosition.Y + 1));
                    AllowedMovement.Remove(((int)result, CurrentPosition.Y + 2));
                }
            }
            // If player piece is (x+1,y+1) or (x-1,y-1) then allow that movement
        }
    }
    public class Rook : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public override void CalculateValidMoves()
        {
            throw new NotImplementedException();
        }

        public override void MovePiece(Game game, Player player)
        {
            throw new NotImplementedException();
        }
    }
    public class Knight : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public override void CalculateValidMoves()
        {
            throw new NotImplementedException();
        }

        public override void MovePiece(Game game, Player player)
        {
            throw new NotImplementedException();
        }
    }
    public class Bishop : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public override void CalculateValidMoves()
        {
            throw new NotImplementedException();
        }

        public override void MovePiece(Game game, Player player)
        {
            throw new NotImplementedException();
        }
    }
    public class Queen : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public override void CalculateValidMoves()
        {
            throw new NotImplementedException();
        }

        public override void MovePiece(Game game, Player player)
        {
            throw new NotImplementedException();
        }
    }
    public class King : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public override void CalculateValidMoves()
        {
            throw new NotImplementedException();
        }

        public override void MovePiece(Game game, Player player)
        {
            throw new NotImplementedException();
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
                Enum.TryParse<ChessCoordinate>(piece.CurrentPosition.X, out ChessCoordinate result);
                Board.Matrix[(int)result, piece.CurrentPosition.Y] = piece;
                Console.WriteLine(Board.Matrix[(int)result, piece.CurrentPosition.Y]);
            }

        }
        if (PlayerTwo != null)
        {
            foreach (var piece in PlayerTwo.GamePieces)
            {
                Enum.TryParse<ChessCoordinate>(piece.CurrentPosition.X, out ChessCoordinate result);
                Board.Matrix[(int)result, piece.CurrentPosition.Y] = piece;
            }
        }
        UpdateGame();
        foreach (var piece in PlayerOne.GamePieces)
        {
            piece.CalculateValidMoves();
        }
        foreach (var piece in PlayerTwo.GamePieces)
        {
            piece.CalculateValidMoves();
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
