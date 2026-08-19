namespace BuildMart.Domain.Enums;

/// <summary>
/// Unit of measure used for construction materials sold by
/// weight/volume rather than by piece (e.g. cement bags, paint cans).
/// </summary>
public enum MeasurementUnit
{
    Piece = 0,
    Kilogram = 1,
    Liter = 2,
    Meter = 3,
    Box = 4,
    Bag = 5,
    Set = 6
}
