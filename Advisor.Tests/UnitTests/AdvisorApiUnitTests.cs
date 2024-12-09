using Advisor.API.DTOs;
using Advisor.Domain.Models;
using Advisor.Services.Models;
using AutoMapper;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Advisor.Tests.UnitTests;

public class AdvisorApiUnitTests
{
    private readonly Mock<IAdvisorQuery> _mockAdvisorQuery;
    private readonly Mock<IAdvisorCommand> _mockAdvisorCommand;
    private readonly Mock<IMapper> _mockMapper;

    public AdvisorApiUnitTests()
    {
        _mockAdvisorQuery = new Mock<IAdvisorQuery>();
        _mockAdvisorCommand = new Mock<IAdvisorCommand>();
        _mockMapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task GetAdvisors_ReturnsOk_WithMappedDtos()
    {
        // Arrange
        var advisors = new List<AdvisorProfile>
        {
            new AdvisorProfile { FullName = "John Doe" },
            new AdvisorProfile { FullName = "Jane Smith" }
        };

        var responseDtos = new List<AdvisorProfileResponseDto>
        {
            new AdvisorProfileResponseDto { FullName = "John Doe" },
            new AdvisorProfileResponseDto { FullName = "Jane Smith" }
        };

        _mockAdvisorQuery.Setup(s => s.GetAdvisorsAsync()).ReturnsAsync(advisors);
        _mockMapper.Setup(m => m.Map<IEnumerable<AdvisorProfileResponseDto>>(advisors)).Returns(responseDtos);

        // Act
        var result = await AdvisorApi.GetAdvisors(_mockAdvisorQuery.Object, _mockMapper.Object);

        // Assert
        var okResult = Assert.IsType<Ok<IEnumerable<AdvisorProfileResponseDto>>>(result);
        var returnValue = okResult.Value;

        Assert.NotNull(returnValue);
        Assert.Equal(2, returnValue.Count());
        _mockAdvisorQuery.Verify(s => s.GetAdvisorsAsync(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<AdvisorProfileResponseDto>>(advisors), Times.Once);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorQuery.Setup(s => s.GetAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await AdvisorApi.GetAdvisor(Guid.NewGuid(), _mockAdvisorQuery.Object, _mockMapper.Object);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task GetAdvisor_ReturnsOk_WithMappedDto()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe" };
        var responseDto = new AdvisorProfileResponseDto { FullName = "John Doe" };

        _mockAdvisorQuery.Setup(s => s.GetAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync(advisor);
        _mockMapper.Setup(m => m.Map<AdvisorProfileResponseDto>(advisor)).Returns(responseDto);

        // Act
        var result = await AdvisorApi.GetAdvisor(Guid.NewGuid(), _mockAdvisorQuery.Object, _mockMapper.Object);

        // Assert
        var okResult = Assert.IsType<Ok<AdvisorProfileResponseDto>>(result);
        var returnValue = okResult.Value;

        Assert.NotNull(returnValue);
        Assert.Equal("John Doe", returnValue.FullName);
    }

    [Fact]
    public async Task CreateAdvisor_ReturnsCreated_WithMappedDto()
    {
        // Arrange
        var requestDto = new AdvisorProfileRequestDto { FullName = "John Doe" };
        var advisor = new AdvisorProfile { FullName = "John Doe" };
        var responseDto = new AdvisorProfileResponseDto { FullName = "John Doe" };

        _mockMapper.Setup(m => m.Map<AdvisorProfile>(requestDto)).Returns(advisor);
        _mockAdvisorCommand.Setup(s => s.CreateAdvisorAsync(It.IsAny<AdvisorProfile>())).ReturnsAsync(advisor);
        _mockMapper.Setup(m => m.Map<AdvisorProfileResponseDto>(advisor)).Returns(responseDto);

        // Act
        var result = await AdvisorApi.CreateAdvisor(requestDto, _mockAdvisorCommand.Object, _mockMapper.Object);

        // Assert
        var createdResult = Assert.IsType<Created<AdvisorProfileResponseDto>>(result);
        var returnValue = createdResult.Value;

        Assert.NotNull(returnValue);
        Assert.Equal("John Doe", returnValue.FullName);

        _mockMapper.Verify(m => m.Map<AdvisorProfile>(requestDto), Times.Once);
        _mockAdvisorCommand.Verify(s => s.CreateAdvisorAsync(advisor), Times.Once);
    }

    [Fact]
    public async Task UpdateAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        var requestDto = new AdvisorProfileRequestDto();
        _mockAdvisorCommand.Setup(s => s.UpdateAdvisorAsync(It.IsAny<Guid>(), It.IsAny<AdvisorProfile>()))
                           .ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await AdvisorApi.UpdateAdvisor(Guid.NewGuid(), requestDto, _mockAdvisorCommand.Object, _mockMapper.Object);

        // Assert
        //Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.NotFound<object>>(result);
        Assert.IsType<NotFound<string>>(result);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsOk_WhenDeletedSuccessfully()
    {
        // Arrange
        var advisor = new AdvisorProfile { FullName = "John Doe" };
        var responseDto = new AdvisorProfileResponseDto { FullName = "John Doe" };

        _mockAdvisorCommand.Setup(s => s.DeleteAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync(advisor);
        _mockMapper.Setup(m => m.Map<AdvisorProfileResponseDto>(advisor)).Returns(responseDto);

        // Act
        var result = await AdvisorApi.DeleteAdvisor(Guid.NewGuid(), _mockAdvisorCommand.Object, _mockMapper.Object);

        // Assert
        var okResult = Assert.IsType<Ok<AdvisorProfileResponseDto>>(result);
        var returnValue = okResult.Value;

        Assert.NotNull(returnValue);
        Assert.Equal("John Doe", returnValue.FullName);
    }

    [Fact]
    public async Task DeleteAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
    {
        // Arrange
        _mockAdvisorCommand.Setup(s => s.DeleteAdvisorAsync(It.IsAny<Guid>())).ReturnsAsync((AdvisorProfile)null);

        // Act
        var result = await AdvisorApi.DeleteAdvisor(Guid.NewGuid(), _mockAdvisorCommand.Object, _mockMapper.Object);

        // Assert
        Assert.IsType<NotFound<string>>(result);
    }
}
