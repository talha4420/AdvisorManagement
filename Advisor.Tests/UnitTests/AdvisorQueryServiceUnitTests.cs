using Advisor.Core.Repositories;
using Advisor.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Advisor.Tests.UnitTests;

public class AdvisorQueryServiceUnitTests
{
    private readonly Mock<IDBRepository<AdvisorProfile>> _mockRepository;
    private readonly ILogger<AdvisorQueryService> _logger;
    private readonly AdvisorQueryService _service;

    public AdvisorQueryServiceUnitTests()
    {
        _mockRepository = new Mock<IDBRepository<AdvisorProfile>>();
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AdvisorQueryService>();
        _service = new AdvisorQueryService(_mockRepository.Object, _logger);
    }

    [Fact]
    public async Task GetAdvisorsAsync_ReturnsAllAdvisors()
    {
        // Arrange
        var advisors = new List<AdvisorProfile>
        {
            new AdvisorProfile {Id = Guid.NewGuid(), FullName = "John Doe", SIN = "123456789" },
            new AdvisorProfile { Id =Guid.NewGuid(), FullName = "Jane Smith", SIN = "987654321" }
        };
        _mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(advisors);

        // Act
        var result = await _service.GetAdvisorsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Equal("John Doe", result.First().FullName);
        Assert.Equal("Jane Smith", result.Last().FullName);
    }

    [Fact]
    public async Task GetAdvisorAsync_ReturnsAdvisor_WhenAdvisorExists()
    {
        // Arrange
        var Id = Guid.NewGuid();
        var advisor = new AdvisorProfile { Id = Id, FullName = "John Doe", SIN = "123456789" };
        _mockRepository.Setup(repo => repo.GetAsync(Id)).ReturnsAsync(advisor);

        // Act
        var result = await _service.GetAdvisorAsync(Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task GetAdvisorAsync_ReturnsNull_WhenAdvisorDoesNotExist()
    {
        // Arrange
        var Id = Guid.NewGuid();
        _mockRepository.Setup(repo => repo.GetAsync(Id)).ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await _service.GetAdvisorAsync(Id);

        // Assert
        Assert.Null(result);
    }
    
}