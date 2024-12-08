namespace Chess_Final.Chess;

using Generics;

public class Pawn : ChessPiece
{
    public override PieceType Type { get; set; } = PieceType.pawn;
    public override (string X, int Y) CurrentPosition { get; set; }
    public override Owner owner { get; init; }
    public Pawn(Owner owner, (string X, int Y) currentPosition)
    {
        this.owner = owner;
        CurrentPosition = (currentPosition.X, currentPosition.Y);
    }
    public override void CalculateValidMoves(Func<int, int, GamePiece?> FindOpponent)
    {
        // Reset AllowedMoves
        AllowedMovement = new();
        // Parse CurrentPosition
        (int CurrentX, int CurrentY) = Chess.ParsePosition(CurrentPosition);
        // If Piece is PlayerOne Piece
        if (this.owner == Owner.Player)
        {
            // On First move can move 2 spacese
            if (FirstMove)
            {
                // Set AllowedMove to move 2 spaces
                AllowedMovement.Add((CurrentX, CurrentY - 2));
                AllowedMovement.Add((CurrentX, CurrentPosition.Y - 1));
            }
            else
            {
                // Regular move -- Can move 1 space forward                    
                AllowedMovement.Add((CurrentX, CurrentPosition.Y - 1));
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
            GamePiece Collision1 = FindOpponent(CurrentX, CurrentY + 1);
            GamePiece Collision2 = FindOpponent(CurrentX, CurrentY + 2);

            // On First move can move 2 spacese
            if (FirstMove)
            {
                // Set AllowedMove to move 2 spaces
                if (Collision2 == null)
                {
                    AllowedMovement.Add((CurrentX, CurrentY + 2));
                }
                if (Collision1 == null)
                {
                    AllowedMovement.Add((CurrentX, CurrentPosition.Y + 1));
                }
            }
            else
            {
                // Regular move -- Can move 1 space forward                    
                if (Collision1 == null)
                {
                    AllowedMovement.Add((CurrentX, CurrentPosition.Y + 1));
                }
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
                if (pieceToRight != null && pieceToRight.owner != owner)
                {
                    AllowedMovement.Add((CurrentX + 1, CurrentY + 1));
                }
                // Set allowed movement if opponent piece to left exists
                if (pieceToLeft != null && pieceToLeft.owner != owner)
                {
                    AllowedMovement.Add((CurrentX - 1, CurrentY + 1));
                }
            }
        }
    }

}
