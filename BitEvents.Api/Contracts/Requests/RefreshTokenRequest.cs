namespace BitEvents.Api.Contracts.Requests;

public sealed class RefreshTokenRequest
{
    public required string Token { get; set; }
}