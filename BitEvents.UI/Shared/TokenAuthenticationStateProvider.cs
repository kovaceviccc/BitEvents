using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BitEvents.UI.Shared;

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _client;
    private ClaimsPrincipal? _user;

    public TokenAuthenticationStateProvider(HttpClient client)
    {
        _client = client;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("Ovde sam user je: " + JsonSerializer.Serialize(_user));
        return _user is null 
            ? new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())) 
            : new AuthenticationState(_user);
    }

    public async Task Login()
    {
        var result = await _client.PostAsJsonAsync("https://localhost:7123/authenticate", new { Email = "admin@test.rs", Password = "Sifra.1234"});
        var response = await result.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        _user = await GetUserStateAsync(response);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<ClaimsPrincipal> GetUserStateAsync(Dictionary<string, string> user)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                user.Select(x => new Claim(x.Key, x.Value))));
    }
}