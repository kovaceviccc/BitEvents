namespace BitEvents.UI.Models.Responses;

public sealed class ValidationFailureResponse
{
    public List<string> Errors { get; init; } = new();
}