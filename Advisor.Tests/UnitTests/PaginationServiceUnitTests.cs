using Advisor.Core.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Advisor.Tests.UnitTests;
public class PaginationServiceUnitTests : IDisposable
{
    private readonly PaginationService _paginationService;
    private readonly TestDbContext _context;

    public PaginationServiceUnitTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _paginationService = new PaginationService();

        // Seed data
        _context.Entities.AddRange(
            new TestEntity { Id = 1, Name = "Entity1" },
            new TestEntity { Id = 2, Name = "Entity2" },
            new TestEntity { Id = 3, Name = "Entity3" },
            new TestEntity { Id = 4, Name = "Entity4" },
            new TestEntity { Id = 5, Name = "Entity5" }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task PaginateAsync_ReturnsCorrectPage()
    {
        // Arrange
        var query = _context.Entities.AsQueryable();
        var pageNumber = 2;
        var pageSize = 2;

        // Act
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // Assert
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(5, result.TotalRecords);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal("Entity3", result.Items.First().Name);
        Assert.Equal("Entity4", result.Items.Last().Name);
    }

    [Fact]
    public async Task PaginateAsync_ReturnsEmpty_WhenNoData()
    {
        // Arrange
        _context.Entities.RemoveRange(_context.Entities); // Clear all seeded data
        await _context.SaveChangesAsync(); // Persist the changes to the in-memory database

        var query = _context.Entities.AsQueryable();
        var pageNumber = 1;
        var pageSize = 2;

        // Act
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // Assert
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalRecords);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(2, result.PageSize);
    }

    [Fact]
    public async Task PaginateAsync_ReturnsFirstPage_WhenPageNumberIsOne()
    {
        // Arrange
        var query = _context.Entities.AsQueryable();
        var pageNumber = 1;
        var pageSize = 3;

        // Act
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // Assert
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(5, result.TotalRecords);
        Assert.Equal("Entity1", result.Items.First().Name);
        Assert.Equal("Entity3", result.Items.Last().Name);
    }

     [Fact]
    public async Task PaginateAsync_ReturnsRemainingRecords_WhenLastPage()
    {
        // Arrange
        var query = _context.Entities.AsQueryable();
        var pageNumber = 3;
        var pageSize = 2;

        // Act
        var result = await _paginationService.PaginateAsync(query, pageNumber, pageSize);

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(5, result.TotalRecords);
        Assert.Equal("Entity5", result.Items.First().Name);
    }

    // Dispose of the context
    public void Dispose()
    {
        _context.Dispose();
    }
}

// DbContext for testing
public class TestDbContext : DbContext
{
    public DbSet<TestEntity> Entities { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }
}

// Entity for testing
public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}
