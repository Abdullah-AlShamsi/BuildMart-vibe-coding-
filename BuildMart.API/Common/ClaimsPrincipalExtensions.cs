using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BuildMart.API.Common;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Reads the authenticated user's id from the JWT "sub" claim.</summary>
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("User id claim not found on the current principal.");
}
