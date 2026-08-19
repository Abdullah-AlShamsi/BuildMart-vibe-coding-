namespace BuildMart.Domain.Exceptions;

/// <summary>Thrown for invalid business input. Mapped to HTTP 400.</summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
