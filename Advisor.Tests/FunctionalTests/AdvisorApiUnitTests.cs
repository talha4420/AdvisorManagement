using System.Net.Http.Json;
using Advisor.Domain.Models;
using Advisor.Services.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;

namespace Advisor.Tests.FunctionalTests;

public class AdvisorApiFunctionalTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IAdvisorQuery> _mockAdvisorQuery;
    private readonly Mock<IAdvisorCommand> _mockAdvisorCommand;

    public AdvisorApiFunctionalTests(WebApplicationFactory<Program> factory)
    {
        _mockAdvisorQuery = new Mock<IAdvisorQuery>();
        _mockAdvisorCommand = new Mock<IAdvisorCommand>();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(_mockAdvisorQuery.Object);
                services.AddSingleton(_mockAdvisorCommand.Object);
            });
        });
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

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/v1/advisors");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<AdvisorProfile>>();
        Assert.Equal(2, result.Count);
        Assert.Equal("John Doe", result[0].FullName);
        Assert.Equal("Jane Smith", result[1].FullName);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsOk_WhenAdvisorExists()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorQuery.Setup(service => service.GetAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync(advisor);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/v1/advisors/{Guid.NewGuid()}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AdvisorProfile>();
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorQuery.Setup(service => service.GetAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync((AdvisorProfile)null);

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/v1/advisors/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAdvisor_ReturnsCreated()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorCommand.Setup(service => service.CreateAdvisorAsync(It.IsAny<AdvisorProfile>())).ReturnsAsync(advisor);

        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/advisors", advisor);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<AdvisorProfile>();
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task UpdateAdvisor_ReturnsOk_WhenAdvisorExists()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorCommand.Setup(service => service.UpdateAdvisorAsync(It.IsAny<Guid>(), It.IsAny<AdvisorProfile>())).ReturnsAsync(advisor);

        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/advisors/{Guid.NewGuid()}", advisor);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AdvisorProfile>();
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task UpdateAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorCommand.Setup(service => service.UpdateAdvisorAsync(It.IsAny<Guid>(), It.IsAny<AdvisorProfile>())).ReturnsAsync((AdvisorProfile)null);

        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"/api/v1/advisors/{Guid.NewGuid()}", new AdvisorProfile());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsOk_WhenAdvisorExists()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe", SIN = "123456789" };
        _mockAdvisorCommand.Setup(service => service.DeleteAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync(advisor);

        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/advisors/{Guid.NewGuid()}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AdvisorProfile>();
        Assert.Equal("John Doe", result.FullName);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorCommand.Setup(service => service.DeleteAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync((AdvisorProfile)null);

        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/v1/advisors/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}