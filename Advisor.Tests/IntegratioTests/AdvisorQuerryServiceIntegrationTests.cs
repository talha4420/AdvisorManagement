using Advisor.Core.DBContexts;
using Advisor.Core.Repositories;
using Advisor.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Advisor.Tests.IntegrationTests;
public class AdvisorQueryServiceIntegrationTests
{
    private readonly AdvisorDBContext _context;
    private readonly DBRepository<AdvisorProfile, AdvisorDBContext> _repository;
    private readonly AdvisorQueryService _service;

    public AdvisorQueryServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AdvisorDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AdvisorDBContext(options);
        _repository = new DBRepository<AdvisorProfile, AdvisorDBContext>(_context);
        _service = new AdvisorQueryService(_repository);
    }

    [Fact]
    public async Task GetAdvisorsAsync_ReturnsAllAdvisors()
    {
        // Arrange
        var advisors = new List<AdvisorProfile>
        {
            new AdvisorProfile {  FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" },
            new AdvisorProfile {  FullName = "Jane Smith", SIN = "987654321", Address = "456 Main St", PhoneNumber = "123-456-7890" }
        };
        await _context.AdvisorProfiles.AddRangeAsync(advisors);
        await _context.SaveChangesAsync();

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
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" };
        await _context.AdvisorProfiles.AddAsync(advisor);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAdvisorAsync(advisor.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task GetAdvisorAsync_ReturnsNull_WhenAdvisorDoesNotExist()
    {
        // Act
        var result = await _service.GetAdvisorAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}