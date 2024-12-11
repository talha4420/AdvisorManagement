using Advisor.API.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data.Common;
using System.Text.Json;
using Advisor.Tests.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Advisor.Tests.UnitTests.ExceptionHandlerUnitTests;
public class ValidationExceptionHandlerUnitTests
{
    private readonly ValidationExceptionHandler _handler;
    private readonly List<string> _logMessages;

    public ValidationExceptionHandlerUnitTests()
    {
        _logMessages = new List<string>();

        // Create a real logger that writes to an in-memory list
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new InMemoryLoggerProvider(_logMessages));
        });

        var logger = loggerFactory.CreateLogger<ValidationExceptionHandler>();

        _handler = new ValidationExceptionHandler(logger);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandle_ValidationException()
    {
        // Arrange
        var validationException = new Mock<ValidationException>();
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        validationException.SetupGet(e => e.Message).Returns("Validation error message");

        // Act
        var result = await _handler.TryHandleAsync(context, validationException.Object, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.Equal("Validation Error", response?.Title);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, response?.Status);
        Assert.Equal("Validation error message", response?.Detail);

        // Verify log output
        Assert.Contains("Validation exception occurred", _logMessages[0]);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandle_InnerValidationException()
    {
        // Arrange
        var validationException = new Mock<ValidationException>();
        var exception = new Exception("Wrapper Exception", validationException.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result); // Ensure the handler handled the exception
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.Equal("Validation Error", response?.Title);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, response?.Status);
        Assert.Equal("Wrapper Exception", response?.Detail);

        // Verify log output
        Assert.Contains("Validation exception occurred", _logMessages[0]);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldNotHandle_NonValidationException()
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
