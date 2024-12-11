using Advisor.API.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data.Common;
using System.Text.Json;
using Advisor.Tests.Helpers;

namespace Advisor.Tests.UnitTests.ExceptionHandlerUnitTests;
public class DatabaseExceptionHandlerUnitTests
{
    private readonly DatabaseExceptionHandler _handler;
    private readonly List<string> _logMessages;

    public DatabaseExceptionHandlerUnitTests()
    {
        _logMessages = new List<string>();

        // Create a real logger that writes to an in-memory list
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new InMemoryLoggerProvider(_logMessages));
        });

        var logger = loggerFactory.CreateLogger<DatabaseExceptionHandler>();

        _handler = new DatabaseExceptionHandler(logger);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandle_DbException()
    {
        // Arrange
        var dbException = new Mock<DbException>().Object;
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var result = await _handler.TryHandleAsync(context, dbException, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.Equal("Database Error", response?.Title);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, response?.Status);
        Assert.Equal("A database error occurred. Please contact support.", response?.Detail);

        // Verify log output
        Assert.Contains("Database exception occurred", _logMessages[0]);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldHandle_InnerDbException()
    {
        // Arrange
        var dbException = new Mock<DbException>().Object;
        var exception = new Exception("Wrapper Exception", dbException);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        var result = await _handler.TryHandleAsync(context, exception, CancellationToken.None);

        // Assert
        Assert.True(result); // Ensure the handler handled the exception
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, context.Response.StatusCode);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var response = await JsonSerializer.DeserializeAsync<ProblemDetails>(context.Response.Body);

        Assert.Equal("Database Error", response?.Title);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, response?.Status);
        Assert.Equal("A database error occurred. Please contact support.", response?.Detail);

        // Verify log output
        Assert.Contains("Database exception occurred", _logMessages[0]);
    }

    [Fact]
    public async Task TryHandleAsync_ShouldNotHandle_NonDbException()
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
