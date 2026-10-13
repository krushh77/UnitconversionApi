using UnitConversion.Api.Infrastructure;
using UnitConversion.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ConversionExceptionHandler>();
builder.Services.AddHealthChecks();

// The catalog is stateless and read-only, so a singleton is safe.
// To move units into a database later, only this registration changes.
builder.Services.AddSingleton<IUnitCatalog, InMemoryUnitCatalog>();
builder.Services.AddSingleton<IConversionService, ConversionService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // OpenAPI document at /openapi/v1.json
}

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Required so the integration tests can reference the entry point.
public partial class Program;
