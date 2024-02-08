namespace BitEvents.Api.Exceptions;

public sealed class DatabaseException : Exception
{
    public DatabaseException()
    {
    }

    public DatabaseException(string message) : base(message)
    {
    }
}