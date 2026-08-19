namespace BuildMart.Domain.Enums;

/// <summary>
/// Application-level roles. Mirrors the role names registered
/// with ASP.NET Core Identity ("Customer", "Admin").
/// </summary>
public enum UserRole
{
    Customer = 0,
    Admin = 1
}
