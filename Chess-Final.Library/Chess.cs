namespace Chess_Final.Chess;

using Player;
using Generics;
using TC_DataManager;
using System.Runtime.CompilerServices;

public class ChessPieces
{

    public class Pawn : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public (string X, int Y) CurrentPosition { get; set; }

    }
    public class Rook : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public (string X, int Y) CurrentPosition { get; set; }

    }
    public class Knight : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public (string X, int Y) CurrentPosition { get; set; }

    }
    public class Bishop : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public (string X, int Y) CurrentPosition { get; set; }

    }
    public class Queen : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public (string X, int Y) CurrentPosition { get; set; }

    }
    public class King : GamePiece
    {
        public PieceType Type { get; set; }
        public (int X, int Y) AllowedMovement { get; set; }
        public bool CanMove { get; set; }
        public (string X, int Y) CurrentPosition { get; set; }

    }
}

public class Chess : Game
{
    public static Chess CurrentGame { get; set; } = new Chess();
    public Chess() : base(GameType.Chess)
    {
        PlayerOne = new Player("PlayerOne");
        PlayerTwo = new Player("PlayerTwo");
    }

    public string Name { get; private set; } = "Chess";
    public GameBoard Board { get; set; }
    public void LayoutGamePieces()
    {
        // var data = DataManager.LoadFile<IEnumerable<PlayerData>>("../../../../Chess-Final.Library/ChessLayout.json");
        IEnumerable<PlayerData> data = DataManager.LoadFile<IEnumerable<PlayerData>>("../../../ChessLayout.json");

        if (data != null)
        {

            var playerPieces = data.Where(p => p.playerType == "player")
            .Select(d => d.data)
            .SelectMany(p => p)
            .SelectMany(g => g.pieces.Select(piece =>
        {
            GamePiece newPiece = g.pieceType switch
            {
                "pawns" => new ChessPieces.Pawn { Type = PieceType.pawn, CurrentPosition = (piece.x, piece.y) },
                "rooks" => new ChessPieces.Rook { Type = PieceType.rook, CurrentPosition = (piece.x, piece.y) },
                "knights" => new ChessPieces.Knight { Type = PieceType.knight, CurrentPosition = (piece.x, piece.y) },
                "bishops" => new ChessPieces.Bishop { Type = PieceType.bishop, CurrentPosition = (piece.x, piece.y) },
                "queen" => new ChessPieces.Queen { Type = PieceType.queen, CurrentPosition = (piece.x, piece.y) },
                "king" => new ChessPieces.King { Type = PieceType.king, CurrentPosition = (piece.x, piece.y) },
            };
            return newPiece;
        }
            )
             ).ToList();

            PlayerOne.GamePieces = playerPieces;

            foreach (var piece in PlayerOne.GamePieces)
            {
                Console.WriteLine($"{piece.Name} @ ({piece.CurrentPosition.X},{piece.CurrentPosition.Y})");
            }
        }
    }

    private GamePiece CreateGamePiece(string pieceType, (string x, int y) piece)
    {
        GamePiece newPiece = pieceType switch
        {
            "pawns" => new ChessPieces.Pawn { Type = PieceType.pawn, CurrentPosition = (piece.x, piece.y) },
            "rooks" => new ChessPieces.Rook { Type = PieceType.rook, CurrentPosition = (piece.x, piece.y) },
            "knights" => new ChessPieces.Knight { Type = PieceType.knight, CurrentPosition = (piece.x, piece.y) },
            "bishops" => new ChessPieces.Bishop { Type = PieceType.bishop, CurrentPosition = (piece.x, piece.y) },
            "queen" => new ChessPieces.Queen { Type = PieceType.queen, CurrentPosition = (piece.x, piece.y) },
            "king" => new ChessPieces.King { Type = PieceType.king, CurrentPosition = (piece.x, piece.y) },
        };
        return newPiece;
    }

    public void JoinGame(Player player)
    {

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
