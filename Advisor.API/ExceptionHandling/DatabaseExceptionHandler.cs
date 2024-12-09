using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System.Data.Common;

namespace Advisor.API.ExceptionHandling;
public class DatabaseExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DatabaseExceptionHandler> _logger;

    public DatabaseExceptionHandler(ILogger<DatabaseExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is DbException || exception.InnerException is DbException)
        {
            _logger.LogError(exception, "Database exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Database Error",
                Status = StatusCodes.Status503ServiceUnavailable,
                Detail = "A database error occurred. Please contact support."
            };

            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        return false;
    }
}

