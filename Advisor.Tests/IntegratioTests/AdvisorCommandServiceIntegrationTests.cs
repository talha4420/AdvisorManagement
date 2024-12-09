using Advisor.Core.DBContexts;
using Advisor.Core.Repositories;
using Advisor.Domain.DomainServices;
using Advisor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Advisor.Tests.IntegrationTests;
public class AdvisorCommandServiceIntegrationTests
{
    private readonly DBRepository<AdvisorProfile, AdvisorDBContext> _repository;
    private readonly IHealthStatusGenerator _healthStatusGenerator;
    private readonly AdvisorCommandService _service;
    private readonly AdvisorDBContext _context;
    

    public AdvisorCommandServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AdvisorDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AdvisorDBContext(options);
        _repository = new DBRepository<AdvisorProfile, AdvisorDBContext>(_context);
        _healthStatusGenerator = new HealthStatusGeneratorService();
        _service = new AdvisorCommandService(_repository, _healthStatusGenerator);
    }

    [Fact]
    public async Task CreateAdvisorAsync_CreatesAdvisorWithGeneratedHealthStatus()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" };

        // Act
        var result = await _service.CreateAdvisorAsync(advisor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(advisor.FullName, result.FullName);
        Assert.Equal(advisor.SIN, result.SIN);
        Assert.Equal(advisor.Address, result.Address);
    }

    [Fact]
    public async Task UpdateAdvisorAsync_UpdatesAdvisorWithAdvisorExists()
    {

        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" };

        // Act
        var added = await _service.CreateAdvisorAsync(advisor);
        // Arrange
        var advisorUpdate = new AdvisorProfile { Id = added.Id,  FullName = "John Ben", SIN = "123456780" };

        // Act
        var result = await _service.UpdateAdvisorAsync(added.Id, advisorUpdate);

        // Assert
        Assert.Equal(advisorUpdate.FullName, result.FullName);
        Assert.Equal(advisorUpdate.SIN, result.SIN);
    }

    [Fact]
    public async Task UpdateAdvisorAsync_UpdatesAdvisorWithAdvisorNotExists()
    {

        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" };

        // Act
        var added = await _service.CreateAdvisorAsync(advisor);
        // Arrange

        var Id = Guid.NewGuid();
        var advisorUpdate = new AdvisorProfile { Id = Id,  FullName = "John Ben", SIN = "123456780" };

        // Act
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateAdvisorAsync(Id, advisorUpdate)
        );

        // Verify the exception message
        Assert.Equal($"Entity with Id '{Id}' not found.", exception.Message);
    }

    [Fact]
    public async Task DeleteAdvisorAsync_DeletesAdvisor()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" };   
        var added = await _service.CreateAdvisorAsync(advisor);

        // Assert
        Assert.NotNull(added);
        // Act
        var result = await _service.DeleteAdvisorAsync(added.Id);

        // Assert
        Assert.Equal(advisor.Id, result.Id);
    }
}