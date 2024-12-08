namespace Chess_Final.Chess;

using Player;
using Generics;
using TC_DataManager;
using Chess_Final.Lobby;

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
    public class Rook : GamePiece
    {
        public PieceType Type { get; set; }
        public bool CanMove { get; set; }
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
    public class Knight : GamePiece
    {
        public PieceType Type { get; set; }
        public bool CanMove { get; set; }
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
    public class Bishop : GamePiece
    {
        public PieceType Type { get; set; }
        public bool CanMove { get; set; }
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
            # region Left/Down
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
    public class Queen : GamePiece
    {
        public PieceType Type { get; set; }
        public bool CanMove { get; set; }
        public override (string X, int Y) CurrentPosition { get; set; }
        public override Owner owner { get; init; }
        public Queen(Owner owner, (string X, int Y) currentPosition)
        {
            this.owner = owner;
            CurrentPosition = (currentPosition.X, currentPosition.Y);
        }
        public override void CalculateValidMoves(Func<int, int, GamePiece> FindOpponent)
        {
            // 🫡
            // Reset AllowedMoves
            AllowedMovement = new();
            // Parse CurrentPosition
            (int CurrentX, int CurrentY) = Chess.ParsePosition(CurrentPosition);
            #region Straight Movement
            int MaxX = 7;
            int MaxY = 7;
            #region Can Move up to -7                
            for (int i = CurrentY; i >= 0; i--)
            {
                if (i == CurrentY) continue;
                GamePiece pieceInstance = FindOpponent(CurrentX, i);
                if (pieceInstance != null)
                {
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        break;
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        else { break; }
                    }
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
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        break;
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        else { break; }
                    }
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
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        break;
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        else { break; }
                    }
                }
                else
                {
                    AllowedMovement.Add((i, CurrentY));
                }
            }
            #endregion
            #region Can Move left to -7                
            for (int i = CurrentX; i >= 0; i--)
            {
                if (i == CurrentX) continue;
                GamePiece pieceInstance = FindOpponent(i, CurrentY);
                if (pieceInstance != null)
                {
                    if (this.owner == Owner.Player)
                    {
                        if (pieceInstance.owner == Owner.Opponent)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        break;
                    }
                    if (this.owner == Owner.Opponent)
                    {
                        if (pieceInstance.owner == Owner.Player)
                        {
                            AllowedMovement.Add((CurrentX, i));
                            break;
                        }
                        else { break; }
                    }
                }
                else
                {
                    AllowedMovement.Add((i, CurrentY));
                }
            }
            #endregion
            #endregion
            #region Diagonal Movement
            int maxXR = 7 - CurrentX;
            int maxXL = ((CurrentX - 7) * 1) + 7; //??


            #region  Right/Up
            for (int i = 1; i < maxXR + 1; i++)
            {
                if (CurrentY - i > 0)
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
                if (CurrentY - i > 0)
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
            # region Left/Down
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

            #endregion
        }
    }
    public class King : GamePiece
    {
        public PieceType Type { get; set; }
        public bool CanMove { get; set; }
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
    public override void NewTurn()
    {
        CheckInCheck();
        if (GameOver) GameOverCleanUp();
        Console.WriteLine($"{CurrentPlayer.Username} Has ended their turn.");
        base.NewTurn();
    }
    public void CheckInCheck()
    {
        Player NextPlayer = CurrentPlayer == PlayerOne ? PlayerTwo : PlayerOne;

        Console.WriteLine($"CurrentPlayer: {CurrentPlayer.Username}");
        Console.WriteLine($"NextPlayer: {NextPlayer.Username}");

        GamePiece King = NextPlayer.GamePieces.FirstOrDefault(gp => gp.Name == "King");

        (int X, int Y) KingPosition = ParsePosition(King.CurrentPosition);

        King.CalculateValidMoves(Board.GetPieceFromMatrix);

        List<(int X, int Y)> Threats = GenerateEnemyMoves(King.owner == Owner.Player ? Owner.Opponent : Owner.Player, UUID);

        NextPlayer.Check = Threats.Any(t => t == KingPosition);
        // if(NextPlayer.Check) GameOver = !Threats.Any(t=> King.AllowedMovement.Any(mv => t == mv));
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
            for (int Y = 0; Y < temp.GetLength(0); Y++)
            {
                for (int X = 0; X < temp.GetLength(1); X++)
                {
                    if (temp[X, Y] != null)
                    {
                        if (temp[X, Y].Name != "King")
                        {
                            temp[X, Y].CalculateValidMoves(game.Board.GetPieceFromMatrix);
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
                    piece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                    foreach (var move in piece.AllowedMovement)
                    {
                        EnemyMoves.Add(move);
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
                    piece.CalculateValidMoves(game.Board.GetPieceFromMatrix);
                    foreach (var move in piece.AllowedMovement)
                    {
                        EnemyMoves.Add(move);
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
                    GamePiece newPiece = g.pieceType switch
                    {
                        "pawns" => new ChessPieces.Pawn(Owner.Player, (piece.x, piece.y)) { Name = "Pawn", Type = PieceType.pawn, GameID = UUID },
                        "rooks" => new ChessPieces.Rook(Owner.Player, (piece.x, piece.y)) { Name = "Rook", Type = PieceType.rook, GameID = UUID },
                        "knights" => new ChessPieces.Knight(Owner.Player, (piece.x, piece.y)) { Name = "Knight", Type = PieceType.knight, GameID = UUID },
                        "bishops" => new ChessPieces.Bishop(Owner.Player, (piece.x, piece.y)) { Name = "Bishop", Type = PieceType.bishop, GameID = UUID },
                        "queen" => new ChessPieces.Queen(Owner.Player, (piece.x, piece.y)) { Name = "Queen", Type = PieceType.queen, GameID = UUID },
                        "king" => new ChessPieces.King(Owner.Player, (piece.x, piece.y)) { Name = "King", Type = PieceType.king, GameID = UUID },
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
                    "pawns" => new ChessPieces.Pawn(Owner.Opponent, (piece.x, piece.y)) { Name = "Pawn", Type = PieceType.pawn, GameID = UUID },
                    "rooks" => new ChessPieces.Rook(Owner.Opponent, (piece.x, piece.y)) { Name = "Rook", Type = PieceType.rook, GameID = UUID },
                    "knights" => new ChessPieces.Knight(Owner.Opponent, (piece.x, piece.y)) { Name = "Knight", Type = PieceType.knight, GameID = UUID },
                    "bishops" => new ChessPieces.Bishop(Owner.Opponent, (piece.x, piece.y)) { Name = "Bishop", Type = PieceType.bishop, GameID = UUID },
                    "queen" => new ChessPieces.Queen(Owner.Opponent, (piece.x, piece.y)) { Name = "Queen", Type = PieceType.queen, GameID = UUID },
                    "king" => new ChessPieces.King(Owner.Opponent, (piece.x, piece.y)) { Name = "King", Type = PieceType.king, GameID = UUID },
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

public enum PieceType
{
    pawn,
    rook,
    knight,
    bishop,
    queen,
    king
}
