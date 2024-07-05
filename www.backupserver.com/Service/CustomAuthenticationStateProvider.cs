// CustomAuthenticationStateProvider.cs
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Your logic to determine the authentication state
        var identity = new ClaimsIdentity(); // Example: create an empty identity

        var user = _httpContextAccessor.HttpContext.User;
        if (user.Identity.IsAuthenticated)
        {
            identity = user.Identity as ClaimsIdentity;
        }

        var claimsPrincipal = new ClaimsPrincipal(identity);

        return Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    public void MarkUserAsAuthenticated(string token)
    {
        // Example method to mark the user as authenticated based on token
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "username"), // Example: set claims based on token
            new Claim(ClaimTypes.Role, "role")
        };

        var identity = new ClaimsIdentity(claims, "apiauth_type");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }
}
