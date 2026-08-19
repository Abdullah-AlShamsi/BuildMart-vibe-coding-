using BuildMart.Domain.Entities;

namespace BuildMart.Application.Interfaces;

public interface IJwtService
{
    /// <summary>Generates a signed JWT for the given user and returns the token plus its expiry.</summary>
    (string Token, DateTime ExpiresAt) GenerateToken(ApplicationUser user, IList<string> roles);
}
