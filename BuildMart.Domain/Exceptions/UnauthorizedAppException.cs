namespace BuildMart.Domain.Exceptions;

/// <summary>Thrown for authentication failures (bad credentials, expired token). Mapped to HTTP 401.</summary>
public class UnauthorizedAppException : Exception
{
    public UnauthorizedAppException(string message = "Invalid authentication credentials.") : base(message) { }
}
