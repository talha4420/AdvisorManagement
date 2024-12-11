using Advisor.API.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Advisor.Tests.Helpers;

namespace Advisor.Tests.UnitTests.ExceptionHandlerUnitTests;
public class ArgumentNullExceptionHandlerUnitTests
{
    private readonly ArgumentNullExceptionHandler _handler;
    private readonly List<string> _logMessages;

    public ArgumentNullExceptionHandlerUnitTests()
    {
        _logMessages = new List<string>();

        // Create a real logger that writes to an in-memory list
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new InMemoryLoggerProvider(_logMessages));
        });

        var logger = loggerFactory.CreateLogger<ArgumentNullExceptionHandler>();

        _handler = new ArgumentNullExceptionHandler(logger);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandle_ArgumentNullException()
    {
        // Arrange
        var ArgumentNullException = new Mock<ArgumentNullException>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        ArgumentNullException.SetupGet(e => e.Message).Returns("ArgumentNull error message");

        // Act
        var result = await _handler.TryHandleAsync(context, ArgumentNullException.Object, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.Equal("Request Error", response?.Title);
        Assert.Equal(StatusCodes.Status400BadRequest, response?.Status);
        Assert.Equal("ArgumentNull error message", response?.Detail);

        // Verify log output
        Assert.Contains("ArgumentNullException exception occurred", _logMessages[0]);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandle_InnerArgumentNullException()
    {
        // Arrange
        var ArgumentNullException = new Mock<ArgumentNullException>();
        var exception = new Exception("Wrapper Exception", ArgumentNullException.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result); // Ensure the handler handled the exception
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.Equal("Request Error", response?.Title);
        Assert.Equal(StatusCodes.Status400BadRequest, response?.Status);
        Assert.Equal("Wrapper Exception", response?.Detail);

        // Verify log output
        Assert.Contains("ArgumentNullException exception occurred", _logMessages[0]);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldNotHandle_NonArgumentNullException()
    {
        // Arrange
        var exception = new Exception("General exception");
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.False(result); // Ensure the handler did NOT handle the exception
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode); // Response status remains unchanged


    }
}
