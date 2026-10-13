namespace UnitConversion.Api.Contracts;

public sealed record ConvertResponse(string Category, string From, string To, double Value, double Result);

public sealed record UnitResponse(string Code, string Name, string Symbol, IReadOnlyList<string> Aliases);

public sealed record CategoryResponse(string Name, string BaseUnit, IReadOnlyList<UnitResponse> Units);
