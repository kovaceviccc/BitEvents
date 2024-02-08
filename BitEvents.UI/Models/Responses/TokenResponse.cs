namespace BitEvents.UI.Models.Responses;

public sealed class TokenResponse
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}