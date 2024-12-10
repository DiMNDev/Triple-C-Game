namespace Chess_Final.Chess;
using Generics;

//REQ#2.1.2
public class Bishop : ChessPiece
{
    public override PieceType Type { get; set; } = PieceType.bishop;
    public override (string X, int Y) CurrentPosition { get; set; }
    public override Owner owner { get; init; }
    public Bishop(Owner owner, (string X, int Y) currentPosition)
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
        int maxXR = 7 - CurrentX;
        int maxXL = ((CurrentX - 7) * 1) + 7; //??


        #region  Right/Up
        for (int i = 1; i < maxXR + 1; i++)
        {
            if (CurrentY - i >= 0)
            {
                GamePiece pieceInstance = FindOpponent(CurrentX + i, CurrentY - i);
                if (pieceInstance != null)
                {
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX + i, CurrentY - i));
                            break;
                        }
                        else { break; }
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX + i, CurrentY - i));
                            break;
                        }
                        else { break; }
                    }
                }
                else
                {
                    AllowedMovement.Add((CurrentX + i, CurrentY - i));
                }
            }
        }
        #endregion
        #region  Right/Down
        for (int i = 1; i < maxXR + 1; i++)
        {
            if (CurrentY + i < 8)
            {
                GamePiece pieceInstance = FindOpponent(CurrentX + i, CurrentY + i);
                if (pieceInstance != null)
                {
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX + i, CurrentY + i));
                            break;
                        }
                        else { break; }
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX + i, CurrentY + i));
                            break;
                        }
                        else { break; }
                    }
                }
                else
                {
                    AllowedMovement.Add((CurrentX + i, CurrentY + i));
                }
            }
        }
        #endregion
        #region  Left/Up                    
        for (int i = 1; i < maxXL + 1; i++)
        {
            if (CurrentY - i >= 0)
            {
                GamePiece pieceInstance = FindOpponent(CurrentX - i, CurrentY - i);
                if (pieceInstance != null)
                {
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX - i, CurrentY - i));
                            break;
                        }
                        else { break; }
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX - i, CurrentY - i));
                            break;
                        }
                        else { break; }
                    }
                }
                else
                {
                    AllowedMovement.Add((CurrentX - i, CurrentY - i));
                }
            }
        }
        #endregion
        #region Left/Down
        for (int i = 1; i < maxXL + 1; i++)
        {
            if (CurrentY + i < 8)
            {
                GamePiece pieceInstance = FindOpponent(CurrentX - i, CurrentY + i);
                if (pieceInstance != null)
                {
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX - i, CurrentY + i));
                            break;
                        }
                        else { break; }
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX - i, CurrentY + i));
                            break;
                        }
                        else { break; }
                    }
                }
                else
                {
                    AllowedMovement.Add((CurrentX - i, CurrentY + i));
                }
            }
        }
        #endregion

    }
}
