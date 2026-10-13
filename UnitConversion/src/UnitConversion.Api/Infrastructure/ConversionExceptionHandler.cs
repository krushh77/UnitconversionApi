using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnitConversion.Api.Domain;

namespace UnitConversion.Api.Infrastructure;

/// <summary>Turns expected domain errors into RFC 7807 problem responses (HTTP 400).</summary>
public sealed class ConversionExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ConversionException ex)
        {
            return false; // anything else falls through to the default 500 handling
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid conversion request",
                Detail = ex.Message,
                Extensions = { ["code"] = ex.Code },
            },
        });
    }
}
