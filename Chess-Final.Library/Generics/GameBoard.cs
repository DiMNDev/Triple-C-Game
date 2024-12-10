namespace Chess_Final.Generics;

public class GameBoard
{
    public GameType Type { get; init; }
    public (int X, int Y)? BoardSize { get; private set; }
    public GamePiece[,]? Matrix { get; private set; }
    public GamePiece[,]? TempMatrix { get; set; }

    public GameBoard(GameType gameType)
    {
        (int X, int Y) Size = gameType switch
        {
            GameType.Chess => (8, 8),
            GameType.Checkers => (8, 8),
            GameType.ConnectFour => (6, 7),
            _ => throw new GameNotFoundException()
        };
        Type = gameType;
        BoardSize = Size;
        Matrix = new GamePiece[Size.X, Size.Y];
    }
    public GamePiece[,] HardCopy()
    {
        GamePiece[,] temp = new GamePiece[8, 8];
        for (int Y = 0; Y < Matrix.GetLength(0); Y++)
        {
            for (int X = 0; X < Matrix.GetLength(1); X++)
            {
                temp[X, Y] = Matrix[X, Y];
            }
        }
        TempMatrix = temp;
        return temp;
    }

    public GamePiece? GetPieceFromMatrix(int x, int y)
    {
        if (x < Matrix.GetLength(0) && x >= 0 && y < Matrix.GetLength(1) && y >= 0)
        {
            return Matrix[x, y];
        }
        else { return null; }
    }
    public GamePiece? GetPieceFromTempMatrix(int x, int y)
    {
        if (x < TempMatrix.GetLength(0) && x >= 0 && y < TempMatrix.GetLength(1) && y >= 0)
        {
            return TempMatrix[x, y];
        }
        else { return null; }
    }

    public void resetMatrix()
    {
        for (int y = 0; y < Matrix.GetLength(0); y++)
        {
            for (int x = 0; x < Matrix.GetLength(1); x++)
            {
                Matrix[x, y] = null;
            }
        }
    }
}
