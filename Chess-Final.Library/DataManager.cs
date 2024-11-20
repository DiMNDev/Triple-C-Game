namespace TC_DataManager;

using System.Text.Json;
using TC_DataManagerException;

public static class DataManager
{
    public static T? LoadFile<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            var data = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
            Console.WriteLine($"Reading data from: {path}");

            return data;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR");
            throw new DataLoadErrorException("Unable to load file", ex);
        }
    }
}
public class PlayerData
{
    public string playerType { get; set; }

    public IEnumerable<PieceType> data { get; set; }
}

public class PieceType
{
    public string pieceType { get; set; }
    public IEnumerable<Piece> pieces { get; set; }
}

public class Piece
{
    public string x { get; set; }
    public int y { get; set; }
}