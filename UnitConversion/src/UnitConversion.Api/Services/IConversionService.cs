namespace UnitConversion.Api.Services;

public interface IConversionService
{
    ConversionResult Convert(string category, string fromUnit, string toUnit, double value);
}

public sealed record ConversionResult(
    string Category,
    string FromUnit,
    string ToUnit,
    double Value,
    double Result);
