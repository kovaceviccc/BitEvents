namespace BitEvents.Api.Exceptions;

public sealed class InvalidInputException : Exception
{
    public InvalidInputException()
    {
    }

    public InvalidInputException(string message) : base(message)
    {
    }
}