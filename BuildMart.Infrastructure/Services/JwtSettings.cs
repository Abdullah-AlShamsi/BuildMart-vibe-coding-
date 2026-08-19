namespace BuildMart.Infrastructure.Services;

/// <summary>Bound from the "JwtSettings" section of appsettings.json.</summary>
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 60;
}
