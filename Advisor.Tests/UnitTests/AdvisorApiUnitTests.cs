
using Advisor.Domain.Models;
using Advisor.Services.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Advisor.Tests.UnitTests;
public class AdvisorApiUnitTests
{
    private readonly Mock<IAdvisorQuery> _mockAdvisorQuery;
    private readonly Mock<IAdvisorCommand> _mockAdvisorCommand;

    public AdvisorApiUnitTests()
    {
        _mockAdvisorQuery = new Mock<IAdvisorQuery>();
        _mockAdvisorCommand = new Mock<IAdvisorCommand>();
    }

    [Fact]
    public async Task GetAdvisors_ReturnsOk()
    {
        // Arrange
        var advisors = new List<AdvisorProfile>
        {
            new AdvisorProfile { FullName = "John Doe", SIN = "123456789" },
            new AdvisorProfile { FullName = "Jane Smith", SIN = "987654321" }
        };
        _mockAdvisorQuery.Setup(service => service.GetAdvisorsAsync()).ReturnsAsync(advisors);

        // Act
        var result = await AdvisorApi.GetAdvisors(_mockAdvisorQuery.Object);

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<AdvisorProfile>>>(result);
        var returnValue = Assert.IsType<List<AdvisorProfile>>(okResult.Value);
        Assert.Equal(2, returnValue.Count);
        Assert.Equal("John Doe", returnValue[0].FullName);
        Assert.Equal("Jane Smith", returnValue[1].FullName);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsOk_WhenAdvisorExists()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorQuery.Setup(service => service.GetAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync(advisor);

        // Act
        var result = await AdvisorApi.GetAdvisor(Guid.NewGuid(), _mockAdvisorQuery.Object);

        // Assert
        var okResult = Assert.IsType<Ok<AdvisorProfile>>(result);
        var returnValue = Assert.IsType<AdvisorProfile>(okResult.Value);
        Assert.Equal("John Doe", returnValue.FullName);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorQuery.Setup(service => service.GetAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await AdvisorApi.GetAdvisor(Guid.NewGuid(), _mockAdvisorQuery.Object);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task CreateAdvisor_ReturnsCreated()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorCommand.Setup(service => service.CreateAdvisorAsync(It.IsAny<AdvisorProfile>())).ReturnsAsync(advisor);

        // Act
        var result = await AdvisorApi.CreateAdvisor(advisor, _mockAdvisorCommand.Object);

        // Assert
        var createdResult = Assert.IsType<Created<AdvisorProfile>>(result);
        var returnValue = Assert.IsType<AdvisorProfile>(createdResult.Value);
        Assert.Equal("John Doe", returnValue.FullName);
    }

    [Fact]
    public async Task UpdateAdvisor_ReturnsOk_WhenAdvisorExists()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorCommand.Setup(service => service.UpdateAdvisorAsync(It.IsAny<Guid>(), It.IsAny<AdvisorProfile>())).ReturnsAsync(advisor);

        // Act
        var result = await AdvisorApi.UpdateAdvisor(Guid.NewGuid(), advisor, _mockAdvisorCommand.Object);

        // Assert
        var okResult = Assert.IsType<Ok<AdvisorProfile>>(result);
        var returnValue = Assert.IsType<AdvisorProfile>(okResult.Value);
        Assert.Equal("John Doe", returnValue.FullName);
    }

    [Fact]
    public async Task UpdateAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorCommand.Setup(service => service.UpdateAdvisorAsync(It.IsAny<Guid>(), It.IsAny<AdvisorProfile>())).ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await AdvisorApi.UpdateAdvisor(Guid.NewGuid(), new AdvisorProfile(), _mockAdvisorCommand.Object);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsOk_WhenAdvisorExists()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorCommand.Setup(service => service.DeleteAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync(advisor);

        // Act
        var result = await AdvisorApi.DeleteAdvisor(Guid.NewGuid(), _mockAdvisorCommand.Object);

        // Assert
        var okResult = Assert.IsType<Ok<AdvisorProfile>>(result);
        var returnValue = Assert.IsType<AdvisorProfile>(okResult.Value);
        Assert.Equal("John Doe", returnValue.FullName);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorCommand.Setup(service => service.DeleteAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await AdvisorApi.DeleteAdvisor(Guid.NewGuid(), _mockAdvisorCommand.Object);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }
}