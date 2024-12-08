using Advisor.Domain.DomainServices;

namespace Advisor.Tests.UnitTests;
public class HealthStatusGeneratorServiceUnitTests
{
    private readonly HealthStatusGeneratorService _service;

    public HealthStatusGeneratorServiceUnitTests()
    {
        _service = new HealthStatusGeneratorService();
    }

    [Fact]
    public void GenerateHealthStatus_ReturnsValidHealthStatus()
    {
        // Act
        var result = _service.GenerateHealthStatus();

        // Assert
        Assert.True(result == HealthStatus.Green || result == HealthStatus.Yellow || result == HealthStatus.Red);
    }

    [Fact]
    public void GenerateHealthStatus_ReturnsDifferentValues()
    {
        // Act
        var results = new HashSet<HealthStatus>();
        for (int i = 0; i < 100; i++)
        {
            results.Add(_service.GenerateHealthStatus());
        }

        // Assert
        Assert.Contains(HealthStatus.Green, results);
        Assert.Contains(HealthStatus.Yellow, results);
        Assert.Contains(HealthStatus.Red, results);
    }

}