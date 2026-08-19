namespace BuildMart.Domain.Exceptions;

/// <summary>Thrown when a request conflicts with current state (e.g. duplicate SKU, insufficient stock). Mapped to HTTP 409.</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
