namespace Chess_Final.Player;
using Generics;

public class Player : IPlayer
{
    public List<GamePiece> GamePieces { get; set; } = new();
    public string Username { get; init; }
    public int Wins { get; set; } = 0;
    public int Losses { get; set; } = 0;
    public Guid PlayerID { get; set; }
    // Consider making a ChessPlayer class
    public bool Check { get; set; } = false;
    public GamePiece? SelectedPiece { get; set; } = null;
    public event Action? GameHasChanged;
    public event Action? TurnOver;

    public Player(string name, Guid? guid = null)
    {
        Username = name;
        PlayerID = guid ?? Guid.NewGuid();
    }

    public void Select(int X, int Y, Game game, Player player)
    {
        Console.WriteLine($"Gameplayer:{game.CurrentPlayer.Username}");
        Console.WriteLine($"player:{player.Username}");

        if (player.Username == game.CurrentPlayer.Username)
        {
            if (SelectedPiece == null)
            {
                // Set SelectedPiece if selection is a players piece
                Console.WriteLine($"Clicked: ({X},{Y})");
                if (Check)
                {
                    SelectedPiece = GamePieces.FirstOrDefault(p => p.Name == "King");
                    SelectedPiece.AllowedMovement = new();
                    SelectedPiece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                }
                else
                {
                    SelectedPiece = GamePieces.Where(p => p.CurrentPosition == (((ChessCoordinate)X).ToString(), Y)).FirstOrDefault();
                }

                // Calculate valid moves if selection is valid
                if (SelectedPiece != null)
                {
                    if (Check && SelectedPiece.AllowedMovement.Count == 0)
                    {
                        game.Winner = game.CurrentPlayer == game.PlayerOne ? game.PlayerTwo : game.PlayerOne;
                        game.GameOverCleanUp();
                    }
                    SelectedPiece.AllowedMovement = new();
                    SelectedPiece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                }
                Console.WriteLine($"Seletcted: {SelectedPiece}");
            }
            else if (SelectedPiece.CurrentPosition == (((ChessCoordinate)X).ToString(), Y))
            {
                // If SelectedPiece is click. Set SelectedPiece to null
                SelectedPiece = null;
            }
            else if (SelectedPiece.AllowedMovement?.Any(p => p.X == X && p.Y == Y) == true)
            {
                // If Selected is valid move, MovePiece
                MovePiece(X, Y, game);
            }
            else
            {
                // If Selecting a piece when SelectedPiece != null, set SelectedPiece to new selection
                SelectedPiece = GamePieces.Where(p => p.CurrentPosition == (((ChessCoordinate)X).ToString(), Y)).FirstOrDefault();
                if (SelectedPiece != null)
                {
                    SelectedPiece.AllowedMovement = new();
                    SelectedPiece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                }
            }

        }
        GameHasChanged?.Invoke();
    }
    public void MovePiece(int X, int Y, Game game)
    {

        if (SelectedPiece.AllowedMovement.Any(m => (m.X, m.Y) == (X, Y)))
        {
            // Parse SelectedPiece Current Location
            Enum.TryParse<ChessCoordinate>(SelectedPiece.CurrentPosition.X, out ChessCoordinate currentX);
            int CurrentY = SelectedPiece.CurrentPosition.Y;
            // Assign Temp: oldPosition
            (ChessCoordinate X, int Y) oldPosition = (currentX, CurrentY);
            // Set SelectedPiece CurrentPosition to new position
            SelectedPiece.CurrentPosition = (((ChessCoordinate)X).ToString(), Y);
            // Move SelectedPiece in Matrix
            game.Board.Matrix[X, Y] = SelectedPiece;
            // Assign oldPosition in Matrix to null
            game.Board.Matrix[oldPosition.X.GetHashCode(), oldPosition.Y] = null;

            // If FirstMove of SelectedPiece is true -> false
            if (SelectedPiece.FirstMove == true) SelectedPiece.FirstMove = false;

            Console.WriteLine($"{game.CurrentPlayer.Username} Moved {SelectedPiece.Name} to {SelectedPiece.CurrentPosition}");
            // Set SelectedPiece to null
            SelectedPiece = null;
            // Piece Moved -- Turn Over
        }
        TurnOver?.Invoke();
        TurnOver = null;
    }
}
