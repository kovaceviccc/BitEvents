namespace BitEvents.Api.Models;

public sealed class User : ModelBase
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; } = default!;
    public string PasswordSalt { get; set; } = default!;
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    public bool IsBlocked { get; set; } = false;
    public List<string> Roles { get; set; } = new();
    public List<EventPartial> FavouriteEvents { get; set; } = new();
    public List<EventPartial> GoingEvents { get; set; } = new();
}