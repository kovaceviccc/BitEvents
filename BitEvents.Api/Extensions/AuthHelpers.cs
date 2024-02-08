using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BitEvents.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace BitEvents.Api.Extensions;

public static class AuthHelpers
{
    public static bool ValidatePassword(this User user, string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, user.PasswordSalt) == user.PasswordHash;
    }

    public static (string, string) HashPassword(string password)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, salt);
        return (salt, passwordHash);
    }

    public static (string, string) GenerateTokens(this User user, string secret)
    {
        return (GenerateJwtToken(user, DateTime.Now.AddMinutes(20), secret), GenerateRefreshToken());
    }

    private static string GenerateJwtToken(User user,
        DateTime expirationTime,
        string secret)
    {
        var key = Encoding.ASCII.GetBytes(secret);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var claims = new List<Claim>
        {
            new("Id", user.Id)
            // new("Organization", user.OrganizationId)
        };
        claims.AddRange(user.Roles.Select(role => new Claim("Roles", role)));
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expirationTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}