namespace Chess_Final.Chess;
using Generics;
using Player;

public class Pawn : GamePiece
{
    public string Name { get; set; }
    public (int X, int Y) AllowedMovement { get; set; }
    public bool CanMove { get; set; }
    public (int X, int Y) CurrentPosition { get; set; }
}

public class Chess : Game
{
    public Chess() : base(GameType.Chess)
    {
    }
    private string LayoutPath = "git";

    public string Name { get; private set; } = "Chess";
    public GameBoard Board { get; set; }
    public void LayoutGamePieces()
    {

    }
}

internal record ChessPieceLayout
{
    //PlayerOpponent[] player = {get; set;}
}

internal record PlayerOpponent
{

}
//internal record 


public class Rootobject
{
    public Class1[] Property1 { get; set; }
}

public class Class1
{
    public Player[] Player { get; set; }
    public Opponent[] Opponent { get; set; }
}

public class Player
{
    public Pawn[] pawns { get; set; }
    public Rook[] rooks { get; set; }
    public Knight[] knights { get; set; }
    public Bishop[] bishops { get; set; }
    public Queen[] queen { get; set; }
    public King[] king { get; set; }
}

public class Pawn
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Rook
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Knight
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Bishop
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Queen
{
    public string x { get; set; }
    public int y { get; set; }
}

public class King
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Opponent
{
    public Pawn1[] pawns { get; set; }
    public Rook1[] rooks { get; set; }
    public Knight1[] knights { get; set; }
    public Bishop1[] bishops { get; set; }
    public Queen1[] queen { get; set; }
    public King1[] king { get; set; }
}

public class Pawn1
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Rook1
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Knight1
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Bishop1
{
    public string x { get; set; }
    public int y { get; set; }
}

public class Queen1
{
    public string x { get; set; }
    public int y { get; set; }
}

public class King1
{
    public string x { get; set; }
    public int y { get; set; }
}
