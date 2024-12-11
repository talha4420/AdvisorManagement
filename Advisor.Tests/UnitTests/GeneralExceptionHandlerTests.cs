using Advisor.API.ExceptionHandling;
using Advisor.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Advisor.Tests.UnitTests;
public class GeneralExceptionHandlerTests
{
    private readonly GeneralExceptionHandler _handler;
    private readonly List<string> _logMessages;

    public GeneralExceptionHandlerTests()
    {
        _logMessages = new List<string>();

        // Create a real logger with InMemory Logger Provider
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new InMemoryLoggerProvider(_logMessages));
        });

        var logger = loggerFactory.CreateLogger<GeneralExceptionHandler>();

        _handler = new GeneralExceptionHandler(logger);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandleExceptionAndLogError()
    {
        // Arrange
        var exception = new Exception("Test exception occurred.");
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result); // Handler processes the exception
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

        // Verify the ProblemDetails response
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.NotNull(response);
        Assert.Equal("Internal Server Error", response.Title);
        Assert.Equal(StatusCodes.Status500InternalServerError, response.Status);
        Assert.Equal("An unexpected error occurred. Please try again later.", response.Detail);

        // Verify the log message
        Assert.Single(_logMessages); // Only one log entry expected
        Assert.Contains("Exception occurred: Test exception occurred.", _logMessages[0]);
    }
}

