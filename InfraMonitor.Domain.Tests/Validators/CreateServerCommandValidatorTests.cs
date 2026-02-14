using FluentAssertions;
using FluentValidation.TestHelper;
using InfraMonitor.Application.Features.Servers.Commands;

namespace InfraMonitor.Domain.Tests.Validators;

public class CreateServerCommandValidatorTests
{
    private readonly CreateServerCommandValidator _validator;

    public CreateServerCommandValidatorTests()
    {
        _validator = new CreateServerCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: "Test Description"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_FailsValidation()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "",
            Ipaddress: "192.168.1.1",
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_NameExceeds100Characters_FailsValidation()
    {
        // Arrange
        var longName = new string('A', 101);
        var command = new CreateServerCommand(
            Name: longName,
            Ipaddress: "192.168.1.1",
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_NameExactly100Characters_PassesValidation()
    {
        // Arrange
        var exactName = new string('A', 100);
        var command = new CreateServerCommand(
            Name: exactName,
            Ipaddress: "192.168.1.1",
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_InvalidIpAddress_FailsValidation()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "999.999.999.999",
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ipaddress)
            .WithErrorMessage("Invalid IP Address format.");
    }

    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("0.0.0.0")]
    [InlineData("255.255.255.255")]
    [InlineData("10.0.0.1")]
    public void Validate_ValidIpAddress_PassesValidation(string ipAddress)
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: ipAddress,
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Theory]
    [InlineData("256.1.1.1")]
    [InlineData("1.256.1.1")]
    [InlineData("1.1.256.1")]
    [InlineData("1.1.1.256")]
    [InlineData("abc.def.ghi.jkl")]
    [InlineData("192.168.1")]
    [InlineData("192.168.1.1.1")]
    public void Validate_InvalidIpAddressFormats_FailsValidation(string ipAddress)
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: ipAddress,
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Fact]
    public void Validate_NullIpAddress_PassesValidation()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: null,
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Fact]
    public void Validate_EmptyIpAddress_PassesValidation()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "",
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Fact]
    public void Validate_IpAddressExceeds50Characters_FailsValidation()
    {
        // Arrange
        var longIp = new string('1', 51);
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: longIp,
            Description: "Test"
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Fact]
    public void Validate_DescriptionExceeds250Characters_FailsValidation()
    {
        // Arrange
        var longDescription = new string('A', 251);
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: longDescription
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_DescriptionExactly250Characters_PassesValidation()
    {
        // Arrange
        var exactDescription = new string('A', 250);
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: exactDescription
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_NullDescription_PassesValidation()
    {
        // Arrange
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: null
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
    }
}
