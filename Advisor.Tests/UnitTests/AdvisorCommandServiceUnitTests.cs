using Advisor.Core.Repositories;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace Advisor.Tests.UnitTests;
public class AdvisorCommandServiceUnitTests
{
    private readonly Mock<IDBRepository<AdvisorProfile>> _mockRepository;
    private readonly Mock<IHealthStatusGenerator> _mockHealthStatusGenerator;
    private readonly Mock<IModelValidator<AdvisorProfile>> _mockValidator;
    private readonly ILogger<AdvisorCommandService> _logger;
    private readonly AdvisorCommandService _service;

    public AdvisorCommandServiceUnitTests()
    {
        _mockRepository = new Mock<IDBRepository<AdvisorProfile>>();
        _mockValidator = new Mock<IModelValidator<AdvisorProfile>>();
        _mockHealthStatusGenerator = new Mock<IHealthStatusGenerator>();
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AdvisorCommandService>();
        _service = new AdvisorCommandService(_mockRepository.Object, _mockHealthStatusGenerator.Object, _mockValidator.Object, _logger);
    }

    [Fact]
    public async Task CreateAdvisorAsync_CreatesAdvisorWithGeneratedHealthStatus()
    {
        // Arrange
        var advisor = new AdvisorProfile { Id = Guid.NewGuid(), FullName = "John Doe", SIN = "123456789" };
        _mockHealthStatusGenerator.Setup(gen => gen.GenerateHealthStatus()).Returns(HealthStatus.Green);
        _mockRepository.Setup(repo => repo.CreateAsync(advisor)).ReturnsAsync(advisor);

        // Act
        var result = await _service.CreateAdvisorAsync(advisor);

        // Assert
        Assert.Equal(HealthStatus.Green, advisor.HealthStatus);
        _mockRepository.Verify(repo => repo.CreateAsync(advisor), Times.Once);
    }

    [Fact]
    public async Task UpdateAdvisorAsync_UpdatesAdvisorWithGeneratedHealthStatus()
    {
        // Arrange
        var Id = Guid.NewGuid();
        var advisor = new AdvisorProfile { Id = Id, FullName = "John Doe", SIN = "123456789" };
        _mockRepository.Setup(repo => repo.UpdateAsync(Id, advisor)).ReturnsAsync(advisor);

        // Act
        var result = await _service.UpdateAdvisorAsync(Id, advisor);

        // Assert
        _mockRepository.Verify(repo => repo.UpdateAsync(Id, advisor), Times.Once);
    }

    [Fact]
    public async Task DeleteAdvisorAsync_DeletesAdvisor()
    {
        // Arrange
        var Id = Guid.NewGuid();
        var advisor = new AdvisorProfile { Id = Id, FullName = "John Doe", SIN = "123456789" };
        _mockRepository.Setup(repo => repo.DeleteAsync(Id)).ReturnsAsync(advisor);

        // Act
        var result = await _service.DeleteAdvisorAsync(Id);

        // Assert
        Assert.Equal(advisor, result);
        _mockRepository.Verify(repo => repo.DeleteAsync(Id), Times.Once);
    }
}