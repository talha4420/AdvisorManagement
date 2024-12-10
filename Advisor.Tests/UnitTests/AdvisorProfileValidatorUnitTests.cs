using Advisor.Domain.Models;
using Advisor.Domain.DomainServices;
using System.ComponentModel.DataAnnotations;

namespace Advisor.Tests.UnitTests;
public class AdvisorProfileValidatorUnitTests
{
    private readonly AdvisorProfileValidator _validator;

    public AdvisorProfileValidatorUnitTests()
    {
        _validator = new AdvisorProfileValidator();
    }

    [Fact]
    public void Validate_ThrowsArgumentNullException_WhenModelIsNull()
    {
        // Arrange
        AdvisorProfile model = null;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => _validator.Validate(model));
        Assert.Equal("AdvisorProfile cannot be null. (Parameter 'model')", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenFullNameIsNullOrWhiteSpace()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = " " };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("FullName is required and must be less than 255 characters.", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenFullNameIsTooLong()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = new string('A', 256) };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("FullName is required and must be less than 255 characters.", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenSINIsNullOrWhiteSpace()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = "Full Name", SIN = " " };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("SIN is required, must be exactly 9 digits.", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenSINIsLess9Digits()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = "Full Name", SIN = "12345678" };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("SIN is required, must be exactly 9 digits.", exception.Message);
    }

    public void Validate_ThrowsValidationException_WhenSINIsMore9Digits()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = "Full Name", SIN = "1234567899" };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("SIN is required, must be exactly 9 digits.", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenSINContainsNonDigitCharacters()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = "Full Name", SIN = "12345678A" };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("SIN is required, must be exactly 9 digits.", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenAddressIsTooLong()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = "Full Name", SIN = "123456789", Address = new string('A', 256) };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("Address must be less than 255 characters.", exception.Message);
    }

    [Fact]
    public void Validate_ThrowsValidationException_WhenPhoneNumberIsTooShort()
    {
        // Arrange
        var model = new AdvisorProfile { FullName = "Full Name", SIN = "123456789", PhoneNumber = "123456789" };

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => _validator.Validate(model));
        Assert.Equal("PhoneNumber must be 10 characters or more.", exception.Message);
    }

    [Fact]
    public void Validate_DoesNotThrowException_WhenModelIsValid()
    {
        // Arrange
        var model = new AdvisorProfile
        {
            FullName = "John Doe",
            SIN = "123456789",
            Address = "123 Main St",
            PhoneNumber = "1234567890"
        };

        // Act & Assert
        _validator.Validate(model);
    }
}
