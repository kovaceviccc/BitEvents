namespace BitEvents.Api.Models;

public sealed class RefreshToken
{
    public required string Token { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsPasswordReset { get; set; } = false;
}