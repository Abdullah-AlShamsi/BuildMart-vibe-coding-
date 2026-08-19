namespace BuildMart.Domain.Common;

/// <summary>
/// Base class for all domain entities that need an integer surrogate key
/// plus creation/update auditing timestamps.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
