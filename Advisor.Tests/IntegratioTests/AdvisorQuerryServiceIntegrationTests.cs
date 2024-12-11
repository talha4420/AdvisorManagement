using Advisor.Core.DBContexts;
using Advisor.Core.Pagination;
using Advisor.Core.Repositories;
using Advisor.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Advisor.Tests.IntegrationTests;
public class AdvisorQueryServiceIntegrationTests
{
    private readonly AdvisorDBContext _context;
    private readonly DBRepository<AdvisorProfile, AdvisorDBContext> _repository;
    private readonly ILogger<AdvisorQueryService> _logger;
    private readonly AdvisorQueryService _service;
    private readonly IPaginator _paginator;

    public AdvisorQueryServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AdvisorDBContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AdvisorDBContext(options);
        _repository = new DBRepository<AdvisorProfile, AdvisorDBContext>(_context);
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AdvisorQueryService>();
        _paginator = new PaginationService();
        _service = new AdvisorQueryService(_repository, _logger, _paginator);
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
    public async Task GetAdvisorsAsyncWithPage_ReturnsPagedResult()
    {
        // Arrange
        var advisors = new List<AdvisorProfile>
        {
            new AdvisorProfile {  FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "123-456-7890" },
            new AdvisorProfile {  FullName = "Jane Smith", SIN = "987654321", Address = "456 Main St", PhoneNumber = "123-456-7890" },
            new AdvisorProfile {  FullName = "Alice Johnson", SIN = "111111111", Address = "789 Main St", PhoneNumber = "123-456-7890" },
            new AdvisorProfile {  FullName = "Bob Brown", SIN = "222222222", Address = "101 Main St", PhoneNumber = "123-456-7890" }
        };
        await _context.AdvisorProfiles.AddRangeAsync(advisors);
        await _context.SaveChangesAsync();

        // Act
        var result = await _service.GetAdvisorsAsyncWithPage(1,2);

        // Assert
        Assert.Equal(4, result.TotalRecords);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(2, result.Items.Count());
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