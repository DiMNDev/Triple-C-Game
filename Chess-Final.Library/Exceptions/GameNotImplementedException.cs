[Serializable]
public class GameNotImplementedException : Exception
{
    public GameNotImplementedException()
    {
    }

    public GameNotImplementedException(string? message) : base(message)
    {
    }

    public GameNotImplementedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
