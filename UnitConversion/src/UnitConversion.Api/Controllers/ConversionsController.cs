using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Contracts;
using UnitConversion.Api.Domain;
using UnitConversion.Api.Services;

namespace UnitConversion.Api.Controllers;

[ApiController]
[Route("api/v1/conversions")]
[Produces("application/json")]
public sealed class ConversionsController(IConversionService conversions, IUnitCatalog catalog) : ControllerBase
{
    /// <summary>Convert a number from one unit to another.</summary>
    [HttpPost("convert")]
    [ProducesResponseType<ConvertResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponse> Convert([FromBody] ConvertRequest request)
        => Ok(Run(request));

    /// <summary>Same as POST, but via query string (handy for quick browser checks).</summary>
    [HttpGet("convert")]
    [ProducesResponseType<ConvertResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public ActionResult<ConvertResponse> ConvertGet([FromQuery] ConvertRequest request)
        => Ok(Run(request));

    /// <summary>List all categories with their supported units.</summary>
    [HttpGet("categories")]
    public ActionResult<IEnumerable<CategoryResponse>> GetCategories()
        => Ok(catalog.GetCategories().Select(ToResponse).ToList());

    /// <summary>List the units of a single category.</summary>
    [HttpGet("categories/{category}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public ActionResult<CategoryResponse> GetCategory(string category)
    {
        if (!catalog.TryGetCategory(category, out var c))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Category not found",
                Detail = $"Unknown category '{category}'.",
            });
        }

        return Ok(ToResponse(c));
    }

    private ConvertResponse Run(ConvertRequest request)
    {
        var r = conversions.Convert(request.Category, request.From, request.To, request.Value!.Value);
        return new ConvertResponse(r.Category, r.FromUnit, r.ToUnit, r.Value, r.Result);
    }

    private static CategoryResponse ToResponse(UnitCategory c) => new(
        c.Name,
        c.BaseUnitCode,
        c.Units.Select(u => new UnitResponse(u.Code, u.Name, u.Symbol, u.Aliases ?? [])).ToList());
}
