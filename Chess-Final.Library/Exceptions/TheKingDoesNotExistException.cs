[Serializable]
internal class TheKingDoesNotExistException : Exception
{
    public TheKingDoesNotExistException()
    {
    }

    public TheKingDoesNotExistException(string? message) : base(message)
    {
    }

    public TheKingDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
