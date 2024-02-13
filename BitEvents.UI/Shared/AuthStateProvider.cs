using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BitEvents.UI.Models.Responses;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BitEvents.UI.Shared;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _client;
    private readonly ILocalStorageService _localStorage;

    public AuthStateProvider(
        HttpClient client,
        ILocalStorageService localStorage)
    {
        _client = client;
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var jwt = await _localStorage.GetItemAsync<string>(ApiConstants.TokenKey);
        if (string.IsNullOrEmpty(jwt))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        return new AuthenticationState(new ClaimsPrincipal(
            new ClaimsIdentity(GetClaimsFromToken(jwt), "Bearer")));
    }

    private static IEnumerable<Claim> GetClaimsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (jwtToken is null) return Enumerable.Empty<Claim>();

        List<Claim> claims = [];
        
        foreach (var claim in jwtToken.Claims)
        {
            // If the claim type is "Roles", add a new claim with type "role"
            if (claim.Type == "Roles")
            {
                claims.Add(new Claim(ClaimTypes.Role, claim.Value));
            }
            else
            {
                claims.Add(claim);
            }
        }
        return claims;

    }

    public void NotifyAuthState()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    
    public async Task Login(TokenResponse tokenResponse)
    {
        await _localStorage.SetItemAsync(ApiConstants.TokenKey, tokenResponse.Token);
        await _localStorage.SetItemAsync(ApiConstants.RefreshTokenKey, tokenResponse.RefreshToken);
        NotifyAuthState();
    }
    
    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync(ApiConstants.TokenKey);
        await _localStorage.RemoveItemAsync(ApiConstants.RefreshTokenKey);
        NotifyAuthState();
    }
    
    public async Task<string> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(ApiConstants.TokenKey);
    }
}