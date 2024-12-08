namespace Chess_Final.Chess;

using Generics;

public class Rook : ChessPiece
{
    public override PieceType Type { get; set; } = PieceType.rook;
    public override (string X, int Y) CurrentPosition { get; set; }
    public override Owner owner { get; init; }
    public Rook(Owner owner, (string X, int Y) currentPosition)
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
        // CAN BE SIMPLIFIED BY OWNERSHIP
        #region PlayerOne
        if (this.owner == Owner.Player)
        {
            int MaxX = 7;
            int MaxY = 7;
            #region Can Move up to -7                
            for (int i = CurrentY; i >= 0; i--)
            {
                if (i == CurrentY) continue;
                GamePiece pieceInstance = FindOpponent(CurrentX, i);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Opponent)
                    {
                        AllowedMovement.Add((CurrentX, i));
                        break;
                    }
                    break;
                }
                else
                {
                    AllowedMovement.Add((CurrentX, i));
                }
            }
            #endregion
            #region Can Move down to +7                     
            for (int i = CurrentY; i <= MaxY; i++)
            {
                if (i == CurrentY) continue;
                GamePiece pieceInstance = FindOpponent(CurrentX, i);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Opponent)
                    {
                        AllowedMovement.Add((CurrentX, i));
                        break;
                    }
                    break;
                }
                else
                {
                    AllowedMovement.Add((CurrentX, i));
                }
            }
            #endregion
            #region Can Move right to +7                
            for (int i = CurrentX; i <= MaxX; i++)
            {
                if (i == CurrentX) continue;
                GamePiece pieceInstance = FindOpponent(i, CurrentY);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Opponent)
                    {
                        AllowedMovement.Add((i, CurrentY));
                        break;
                    }
                    else if (pieceInstance.owner == Owner.Player)
                    {
                        break;
                    }
                }
                else
                {
                    AllowedMovement.Add((i, CurrentY));
                }
            }
            #endregion
            #region Can Move left to +7                
            for (int i = CurrentX; i >= 0; i--)
            {
                if (i == CurrentX) continue;
                GamePiece pieceInstance = FindOpponent(i, CurrentY);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Opponent)
                    {
                        AllowedMovement.Add((i, CurrentY));
                        break;
                    }
                    break;
                }
                else
                {
                    AllowedMovement.Add((i, CurrentY));
                }
            }
            #endregion
        }
        #endregion
        #region PlayerTwo
        if (this.owner == Owner.Opponent)
        {
            int MaxX = 7;
            int MaxY = 7;
            #region Can Move up to -7                
            for (int i = CurrentY; i >= 0; i--)
            {
                if (i == CurrentY) continue;
                GamePiece pieceInstance = FindOpponent(CurrentX, i);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Player)
                    {
                        AllowedMovement.Add((CurrentX, i));
                        break;
                    }
                    break;
                }
                else
                {
                    AllowedMovement.Add((CurrentX, i));
                }
            }
            #endregion
            #region Can Move down to +7                     
            for (int i = CurrentY; i <= MaxY; i++)
            {
                if (i == CurrentY) continue;
                GamePiece pieceInstance = FindOpponent(CurrentX, i);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Player)
                    {
                        AllowedMovement.Add((CurrentX, i));
                        break;
                    }
                    break;
                }
                else
                {
                    AllowedMovement.Add((CurrentX, i));
                }
            }
            #endregion
            #region Can Move right to +7                
            for (int i = CurrentX; i <= MaxX; i++)
            {
                if (i == CurrentX) continue;
                GamePiece pieceInstance = FindOpponent(i, CurrentY);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Player)
                    {
                        AllowedMovement.Add((i, CurrentY));
                        break;
                    }
                    else if (pieceInstance.owner == Owner.Player)
                    {
                        break;
                    }
                }
                else
                {
                    AllowedMovement.Add((i, CurrentY));
                }
            }
            #endregion
            #region Can Move left to +7                
            for (int i = CurrentX; i >= 0; i--)
            {
                if (i == CurrentX) continue;
                GamePiece pieceInstance = FindOpponent(i, CurrentY);
                if (pieceInstance != null)
                {
                    if (pieceInstance.owner == Owner.Player)
                    {
                        AllowedMovement.Add((i, CurrentY));
                        break;
                    }
                    break;
                }
                else
                {
                    AllowedMovement.Add((i, CurrentY));
                }
            }
            #endregion
        }
        #endregion
    }
}
