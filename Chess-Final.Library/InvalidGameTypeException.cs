namespace Chess_Final.Lobby;

[Serializable]
internal class InvalidGameTypeException : Exception
{
    public InvalidGameTypeException()
    {
    }

    public InvalidGameTypeException(string? message) : base(message)
    {
    }

    public InvalidGameTypeException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}