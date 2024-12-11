using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace Advisor.API.ExceptionHandling;
public class ArgumentNullExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ArgumentNullExceptionHandler> _logger;

    public ArgumentNullExceptionHandler(ILogger<ArgumentNullExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is ArgumentNullException || exception.InnerException is ArgumentNullException)
        {
            _logger.LogError(exception, "ArgumentNullException exception occurred: {Message}", exception.Message);

            var problemDetails = new ProblemDetails
            {
                Title = "Request Error",
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        return false;
    }
}

