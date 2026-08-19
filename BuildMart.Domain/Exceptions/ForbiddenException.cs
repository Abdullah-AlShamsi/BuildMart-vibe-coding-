namespace BuildMart.Domain.Exceptions;

/// <summary>Thrown when an authenticated user is not allowed to perform an action. Mapped to HTTP 403.</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "You are not allowed to perform this action.") : base(message) { }
}
