using UnitConversion.Api.Domain;
using UnitConversion.Api.Services;

namespace UnitConversion.Tests;

public class ConversionServiceTests
{
    private readonly ConversionService _sut = new(new InMemoryUnitCatalog());

    [Theory]
    [InlineData("length", "meter", "foot", 1, 3.280839895)]
    [InlineData("length", "kilometer", "meter", 1, 1000)]
    [InlineData("length", "mile", "kilometer", 1, 1.609344)]
    [InlineData("mass", "kilogram", "pound", 1, 2.20462262185)]
    [InlineData("mass", "gram", "kilogram", 500, 0.5)]
    [InlineData("temperature", "celsius", "fahrenheit", 100, 212)]
    [InlineData("temperature", "celsius", "fahrenheit", 0, 32)]
    [InlineData("temperature", "fahrenheit", "celsius", -40, -40)]
    [InlineData("temperature", "celsius", "kelvin", 0, 273.15)]
    [InlineData("volume", "liter", "us-gallon", 3.785411784, 1)]
    [InlineData("speed", "kilometer-per-hour", "meter-per-second", 36, 10)]
    public void Convert_ReturnsExpectedValue(string category, string from, string to, double value, double expected)
    {
        var result = _sut.Convert(category, from, to, value);

        Assert.Equal(expected, result.Result, precision: 6);
    }

    [Fact]
    public void Convert_IsCaseInsensitive_AndAcceptsSymbolsAndAliases()
    {
        var result = _sut.Convert("LENGTH", "KM", "Feet", 1);

        Assert.Equal("kilometer", result.FromUnit);
        Assert.Equal("foot", result.ToUnit);
    }

    [Fact]
    public void Convert_SameUnit_ReturnsSameValue()
        => Assert.Equal(42.5, _sut.Convert("length", "meter", "meter", 42.5).Result);

    [Fact]
    public void Convert_AbsoluteZero_IsAllowed()
        => Assert.Equal(0, _sut.Convert("temperature", "celsius", "kelvin", -273.15).Result, precision: 6);

    [Theory]
    [InlineData("temperature", "celsius", "kelvin", -300)]
    [InlineData("mass", "kilogram", "gram", -1)]
    public void Convert_BelowPhysicalMinimum_Throws(string category, string from, string to, double value)
        => Assert.Throws<InvalidValueException>(() => _sut.Convert(category, from, to, value));

    [Fact]
    public void Convert_UnknownCategory_Throws()
        => Assert.Throws<UnknownCategoryException>(() => _sut.Convert("color", "a", "b", 1));

    [Fact]
    public void Convert_UnknownUnit_Throws()
        => Assert.Throws<UnknownUnitException>(() => _sut.Convert("length", "meter", "parsec", 1));

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Convert_NonFiniteValue_Throws(double value)
        => Assert.Throws<InvalidValueException>(() => _sut.Convert("length", "meter", "foot", value));

    [Fact]
    public void Catalog_Contains_RequiredCategories()
    {
        var catalog = new InMemoryUnitCatalog();

        Assert.True(catalog.TryGetCategory("length", out _));
        Assert.True(catalog.TryGetCategory("temperature", out _));
        Assert.True(catalog.TryGetCategory("mass", out _));
    }
}
