namespace BitEvents.Api.Contracts.Requests;

public sealed class AuthenticateRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}