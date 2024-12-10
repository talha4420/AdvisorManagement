using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Advisor.API.ExceptionHandling;
public class ValidationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DatabaseExceptionHandler> _logger;

    public ValidationExceptionHandler(ILogger<DatabaseExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ValidationException || exception.InnerException is ValidationException)
        {
            _logger.LogError(exception, "Validation exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Validation Error",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        return false;
    }
}

