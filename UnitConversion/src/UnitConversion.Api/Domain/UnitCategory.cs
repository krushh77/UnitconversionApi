namespace UnitConversion.Api.Domain;

/// <summary>A group of units that can be converted between each other (e.g. length).</summary>
public sealed class UnitCategory
{
    private readonly Dictionary<string, UnitDefinition> _lookup = new(StringComparer.OrdinalIgnoreCase);

    public UnitCategory(string name, string baseUnitCode, IEnumerable<UnitDefinition> units, double? minBaseValue = null)
    {
        Name = name;
        BaseUnitCode = baseUnitCode;
        MinBaseValue = minBaseValue;
        Units = units.ToList();

        foreach (var unit in Units)
        {
            Register(unit.Code, unit);
            Register(unit.Symbol, unit);
            foreach (var alias in unit.Aliases ?? [])
            {
                Register(alias, unit);
            }
        }

        if (!_lookup.ContainsKey(baseUnitCode))
        {
            throw new InvalidOperationException($"Base unit '{baseUnitCode}' is not defined in category '{name}'.");
        }
    }

    public string Name { get; }

    public string BaseUnitCode { get; }

    /// <summary>Physical lower bound expressed in the base unit (e.g. 0 K for temperature), if any.</summary>
    public double? MinBaseValue { get; }

    public IReadOnlyList<UnitDefinition> Units { get; }

    public bool TryGetUnit(string codeOrAlias, out UnitDefinition unit)
        => _lookup.TryGetValue(codeOrAlias.Trim(), out unit!);

    private void Register(string key, UnitDefinition unit)
    {
        if (_lookup.TryGetValue(key, out var existing) && existing != unit)
        {
            throw new InvalidOperationException($"Duplicate unit identifier '{key}' in category '{Name}'.");
        }

        _lookup[key] = unit;
    }
}
