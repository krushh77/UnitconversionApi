using System.ComponentModel.DataAnnotations;

namespace UnitConversion.Api.Contracts;

public sealed class ConvertRequest
{
    /// <example>length</example>
    [Required, StringLength(50)]
    public string Category { get; init; } = string.Empty;

    /// <example>meter</example>
    [Required, StringLength(50)]
    public string From { get; init; } = string.Empty;

    /// <example>foot</example>
    [Required, StringLength(50)]
    public string To { get; init; } = string.Empty;

    /// <example>100</example>
    [Required]
    public double? Value { get; init; }
}
