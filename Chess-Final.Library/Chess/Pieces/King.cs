namespace Chess_Final.Chess;

using Generics;
using Chess_Final.Lobby;

public class King : ChessPiece
{
    public override PieceType Type { get; set; } = PieceType.king;
    public override (string X, int Y) CurrentPosition { get; set; }
    public override Owner owner { get; init; }
    public King(Owner owner, (string X, int Y) currentPosition)
    {
        this.owner = owner;
        CurrentPosition = (currentPosition.X, currentPosition.Y);
    }
    public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
    {
        // Reset AllowedMoves
        AllowedMovement = new();
        // 🫡
        // Parse CurrentPosition
        (int CurrentX, int CurrentY) = Chess.ParsePosition(CurrentPosition);

        // Possible Moves
        (int X, int Y) UP = (CurrentX, CurrentY - 1);
        (int X, int Y) UP_Left = (CurrentX - 1, CurrentY - 1);
        (int X, int Y) UP_Right = (CurrentX + 1, CurrentY - 1);
        (int X, int Y) Left = (CurrentX - 1, CurrentY);
        (int X, int Y) Down_Left = (CurrentX - 1, CurrentY + 1);
        (int X, int Y) Down = (CurrentX, CurrentY + 1);
        (int X, int Y) Down_right = (CurrentX + 1, CurrentY + 1);
        (int X, int Y) Right = (CurrentX + 1, CurrentY);

        List<(int X, int Y)> PossibleMoves = [UP, UP_Left, UP_Right, Left, Down_Left, Down, Down_right, Right];
        // Filter this list to make sure it's all on the board!
        PossibleMoves = PossibleMoves.Where(mv => mv.X > 0 && mv.X < 7 && mv.Y > 0 && mv.Y < 7).ToList();

        List<(int X, int Y)> EnemyMoves = this.owner switch
        {
            Owner.Player => Chess.GenerateEnemyMoves(Owner.Opponent, GameID),
            Owner.Opponent => Chess.GenerateEnemyMoves(Owner.Player, GameID),
        };

        List<(int X, int Y)> PlayerPieces = this.owner switch
        {
            Owner.Player => Chess.GetPlayerPiecePositions(Owner.Player, GameID, this),
            Owner.Opponent => Chess.GetPlayerPiecePositions(Owner.Opponent, GameID, this),
        };

        // Filter out possible moves that would put the king in check
        PossibleMoves = PossibleMoves.Where(mv => !EnemyMoves.Contains(mv)).ToList();
        // Filter out possilbe moves if the players pieces are in the way
        PossibleMoves = PossibleMoves.Where(mv => !PlayerPieces.Contains(mv)).ToList();
        // Include attacks that would put the king in check
        List<(int X, int Y)> AltThreats = Chess.ValidateSafeMovesForKing(owner, PossibleMoves, GameID);
        PossibleMoves = PossibleMoves.Where(mv => !AltThreats.Contains(mv)).ToList();

        Game game = LobbyManager.GetGame(GameType.Chess, GameID);

        AllowedMovement.AddRange(PossibleMoves);
    }
}
