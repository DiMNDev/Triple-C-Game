namespace TC_DataManager;

using System.Text.Json;
using TC_DataManagerException;

public static class DataManager
{
    public static T? LoadFile<T>(string path)
    {
        try
        {
            var data = JsonSerializer.Deserialize<T>(File.ReadAllText($"{path}"));
            Console.WriteLine($"Reading data from: {path}");

            return data;
        }
        catch
        {
            Console.WriteLine("ERROR");
            throw new DataLoadErrorException();
        }
    }
}
public class PlayerData
{
    public List<PieceGroup> player { get; set; }
    public List<PieceGroup> opponent { get; set; }
}

public class PieceGroup
{
    public List<Piece> pawns { get; set; }
    public List<Piece> rooks { get; set; }
    public List<Piece> knights { get; set; }
    public List<Piece> bishops { get; set; }
    public List<Piece> queen { get; set; }
    public List<Piece> king { get; set; }
}

public class Piece
{
    public string x { get; set; }
    public int y { get; set; }
}