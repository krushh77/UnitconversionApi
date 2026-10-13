using UnitConversion.Api.Domain;

namespace UnitConversion.Api.Services;

/// <summary>
/// Source of truth for categories and units. Today it is hardcoded in memory;
/// tomorrow it can be backed by a database or config without touching callers.
/// </summary>
public interface IUnitCatalog
{
    IReadOnlyCollection<UnitCategory> GetCategories();

    bool TryGetCategory(string name, out UnitCategory category);
}
