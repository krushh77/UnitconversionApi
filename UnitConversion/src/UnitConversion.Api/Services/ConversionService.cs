using System.Globalization;
using UnitConversion.Api.Domain;

namespace UnitConversion.Api.Services;

public sealed class ConversionService(IUnitCatalog catalog) : IConversionService
{
    // Hub-and-spoke: every unit knows only how to reach its base unit, so adding
    // N units needs N definitions instead of N*N conversion pairs.
    public ConversionResult Convert(string category, string fromUnit, string toUnit, double value)
    {
        if (!double.IsFinite(value))
        {
            throw new InvalidValueException("Value must be a finite number.");
        }

        if (!catalog.TryGetCategory(category, out var cat))
        {
            throw new UnknownCategoryException(category);
        }

        if (!cat.TryGetUnit(fromUnit, out var from))
        {
            throw new UnknownUnitException(cat.Name, fromUnit);
        }

        if (!cat.TryGetUnit(toUnit, out var to))
        {
            throw new UnknownUnitException(cat.Name, toUnit);
        }

        var baseValue = from.ToBase(value);

        // Small tolerance so -273.15 C (== 0 K up to floating-point noise) is accepted.
        if (cat.MinBaseValue is { } min && baseValue < min - 1e-9)
        {
            throw new InvalidValueException(
                $"Value {value} {from.Code} is below the physical minimum for {cat.Name}.");
        }

        var result = to.FromBase(baseValue);
        return new ConversionResult(cat.Name, from.Code, to.Code, value, Clean(result));
    }

    // Strip floating-point noise (100 C -> 212.00000000000003 F) by keeping 12 significant digits.
    private static double Clean(double value)
        => double.Parse(value.ToString("G12", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
}
