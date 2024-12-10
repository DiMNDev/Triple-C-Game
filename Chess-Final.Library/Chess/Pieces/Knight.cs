namespace Chess_Final.Chess;
using Generics;

//REQ#2.1.2
public class Knight : ChessPiece
{
    public override PieceType Type { get; set; } = PieceType.knight;
    public override (string X, int Y) CurrentPosition { get; set; }
    public override Owner owner { get; init; }
    public Knight(Owner owner, (string X, int Y) currentPosition)
    {
        this.owner = owner;
        CurrentPosition = (currentPosition.X, currentPosition.Y);
    }
    public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
    {
        // Reset AllowedMoves
        AllowedMovement = new();
        // Parse CurrentPosition
        (int CurrentX, int CurrentY) = Chess.ParsePosition(CurrentPosition);

        List<(int vX, int vY)> ValidMoves = [(1, 2), (1, -2), (-1, 2), (-1, -2), (2, 1), (2, -1), (-2, 1), (-2, -1)];
        foreach (var move in ValidMoves)
        {
            if (CurrentX + move.vX >= 0 && CurrentX + move.vX < 8 && CurrentY + move.vY >= 0 && CurrentY + move.vY < 8)
            {
                GamePiece? pieceInstance = FindOpponent(CurrentX + move.vX, CurrentY + move.vY);
                if (this.owner == Owner.Player)
                {
                    if (pieceInstance != null)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX + move.vX, CurrentY + move.vY));
                        }
                    }
                    else
                    {
                        AllowedMovement.Add((CurrentX + move.vX, CurrentY + move.vY));
                    }
                }
                else if (this.owner == Owner.Opponent)
                {
                    if (pieceInstance != null)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX + move.vX, CurrentY + move.vY));
                        }
                    }
                    else
                    {
                        AllowedMovement.Add((CurrentX + move.vX, CurrentY + move.vY));
                    }
                }
            }
        }
    }
}
