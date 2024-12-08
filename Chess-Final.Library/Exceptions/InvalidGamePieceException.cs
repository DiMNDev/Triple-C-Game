
[Serializable]
public class InvalidGamePieceException : Exception
{
    public InvalidGamePieceException()
    {
    }

    public InvalidGamePieceException(string? message) : base(message)
    {
    }

    public InvalidGamePieceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}