using FluentAssertions;
using FluentValidation.TestHelper;
using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Domain.Enums;

namespace InfraMonitor.Domain.Tests.Validators;

public class UpdateServerCommandValidatorTests
{
    private readonly UpdateServerCommandValidator _validator;

    public UpdateServerCommandValidatorTests()
    {
        _validator = new UpdateServerCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: "Test Description",
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_FailsValidation()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "",
            Ipaddress: "192.168.1.1",
            Description: "Test",
            Status: ServerStatus.Up
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
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: longName,
            Ipaddress: "192.168.1.1",
            Description: "Test",
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    [InlineData("172.16.0.1")]
    public void Validate_ValidIpAddress_PassesValidation(string ipAddress)
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: ipAddress,
            Description: "Test",
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Theory]
    [InlineData("999.999.999.999")]
    [InlineData("abc.def.ghi.jkl")]
    [InlineData("192.168.1")]
    public void Validate_InvalidIpAddress_FailsValidation(string ipAddress)
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: ipAddress,
            Description: "Test",
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Ipaddress)
            .WithErrorMessage("Invalid IP Address format.");
    }

    [Fact]
    public void Validate_NullIpAddress_PassesValidation()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: null,
            Description: "Test",
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Ipaddress);
    }

    [Fact]
    public void Validate_DescriptionExceeds250Characters_FailsValidation()
    {
        // Arrange
        var longDescription = new string('A', 251);
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: longDescription,
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Description);
    }

    [Fact]
    public void Validate_NullDescription_PassesValidation()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: null,
            Status: ServerStatus.Up
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Description);
    }
}
