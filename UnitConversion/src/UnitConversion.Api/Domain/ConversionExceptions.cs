namespace UnitConversion.Api.Domain;

/// <summary>Base type for all expected, client-caused conversion errors (mapped to HTTP 400).</summary>
public abstract class ConversionException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

public sealed class UnknownCategoryException(string category)
    : ConversionException("unknown_category", $"Unknown category '{category}'.");

public sealed class UnknownUnitException(string category, string unit)
    : ConversionException("unknown_unit", $"Unit '{unit}' is not supported in category '{category}'.");

public sealed class InvalidValueException(string message)
    : ConversionException("invalid_value", message);
