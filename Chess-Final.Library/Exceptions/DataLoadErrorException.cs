namespace TC_DataManagerException;

using System;

[Serializable]
public class DataLoadErrorException : Exception
{
    public DataLoadErrorException()
    {
    }

    public DataLoadErrorException(string? message) : base(message)
    {
    }

    public DataLoadErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
