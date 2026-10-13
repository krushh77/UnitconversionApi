using UnitConversion.Api.Domain;

namespace UnitConversion.Api.Services;

public sealed class InMemoryUnitCatalog : IUnitCatalog
{
    private readonly Dictionary<string, UnitCategory> _categories;

    public InMemoryUnitCatalog()
    {
        _categories = new(StringComparer.OrdinalIgnoreCase);
        foreach (var category in BuildCategories())
        {
            _categories.Add(category.Name, category);
        }
    }

    public IReadOnlyCollection<UnitCategory> GetCategories() => _categories.Values;

    public bool TryGetCategory(string name, out UnitCategory category)
        => _categories.TryGetValue(name.Trim(), out category!);

    // To add a new unit: add one line below. To add a category: add one UnitCategory.
    private static IEnumerable<UnitCategory> BuildCategories()
    {
        // Base unit: metre
        yield return new UnitCategory("length", "meter",
        [
            new("millimeter", "Millimeter", "mm", 0.001),
            new("centimeter", "Centimeter", "cm", 0.01),
            new("meter", "Meter", "m", 1, Aliases: ["metre"]),
            new("kilometer", "Kilometer", "km", 1000, Aliases: ["kilometre"]),
            new("inch", "Inch", "in", 0.0254),
            new("foot", "Foot", "ft", 0.3048, Aliases: ["feet"]),
            new("yard", "Yard", "yd", 0.9144),
            new("mile", "Mile", "mi", 1609.344),
        ]);

        // Base unit: kilogram
        yield return new UnitCategory("mass", "kilogram",
        [
            new("milligram", "Milligram", "mg", 0.000001),
            new("gram", "Gram", "g", 0.001),
            new("kilogram", "Kilogram", "kg", 1),
            new("tonne", "Metric tonne", "t", 1000),
            new("ounce", "Ounce", "oz", 0.028349523125),
            new("pound", "Pound", "lb", 0.45359237, Aliases: ["lbs"]),
            new("stone", "Stone", "st", 6.35029318),
        ], minBaseValue: 0);

        // Base unit: kelvin. Celsius/Fahrenheit are affine (factor + offset).
        yield return new UnitCategory("temperature", "kelvin",
        [
            new("kelvin", "Kelvin", "K", 1),
            new("celsius", "Degrees Celsius", "C", 1, 273.15),
            new("fahrenheit", "Degrees Fahrenheit", "F", 5.0 / 9.0, 273.15 - 32 * 5.0 / 9.0),
        ], minBaseValue: 0); // absolute zero

        // Base unit: litre
        yield return new UnitCategory("volume", "liter",
        [
            new("milliliter", "Milliliter", "ml", 0.001, Aliases: ["millilitre"]),
            new("liter", "Liter", "l", 1, Aliases: ["litre"]),
            new("cubic-meter", "Cubic meter", "m3", 1000),
            new("us-fluid-ounce", "US fluid ounce", "floz", 0.0295735295625),
            new("us-cup", "US cup", "cup", 0.2365882365),
            new("us-gallon", "US gallon", "gal", 3.785411784),
        ], minBaseValue: 0);

        // Base unit: metre per second
        yield return new UnitCategory("speed", "meter-per-second",
        [
            new("meter-per-second", "Meter per second", "m/s", 1, Aliases: ["mps"]),
            new("kilometer-per-hour", "Kilometer per hour", "km/h", 1 / 3.6, Aliases: ["kph"]),
            new("mile-per-hour", "Mile per hour", "mph", 0.44704),
            new("knot", "Knot", "kn", 1852.0 / 3600.0, Aliases: ["knots"]),
        ]);
    }
}
