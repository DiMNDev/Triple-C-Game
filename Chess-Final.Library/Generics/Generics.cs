namespace Chess_Final.Generics;
public enum JoinAs
{
    Player,
    Spectator
}
public enum GameType
{
    Chess,
    Checkers,
    ConnectFour
}
public enum ChessCoordinate
{
    A, B, C, D, E, F, G, H
}
public enum Owner
{
    Player,
    Opponent
}
public static class FilePaths
{
    public static string Tests { get; } = "../../../../Chess-Final.Library/Data/ChessLayout.json";
    public static string Blazor { get; } = "../Chess-Final.Library/Data/ChessLayout.json";
    public static string Console { get; } = "";
}