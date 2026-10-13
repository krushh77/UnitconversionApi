namespace UnitConversion.Api.Domain;

/// <summary>
/// A unit expressed relative to its category's base unit:
/// <c>baseValue = value * ToBaseFactor + ToBaseOffset</c>.
/// Linear units (metre, kilogram...) only need a factor; affine units
/// (Celsius, Fahrenheit) also need an offset.
/// </summary>
public sealed record UnitDefinition(
    string Code,
    string Name,
    string Symbol,
    double ToBaseFactor,
    double ToBaseOffset = 0,
    IReadOnlyList<string>? Aliases = null)
{
    public double ToBase(double value) => value * ToBaseFactor + ToBaseOffset;

    public double FromBase(double baseValue) => (baseValue - ToBaseOffset) / ToBaseFactor;
}
