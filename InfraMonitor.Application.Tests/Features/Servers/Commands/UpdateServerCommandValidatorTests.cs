using FluentValidation.TestHelper;
using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Domain.Enums;
using Xunit;

namespace InfraMonitor.Application.Tests.Features.Servers.Commands;

public class UpdateServerCommandValidatorTests
{
    private readonly UpdateServerCommandValidator _validator;

    public UpdateServerCommandValidatorTests()
    {
        _validator = new UpdateServerCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Update Server",
            Ipaddress: "192.168.1.1",
            Status: ServerStatus.Up,
            Description: "Update Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_EmptyServerId_ReturnsError()
    {
        // Assuming ServerId should be > 0 or not empty. RuleFor(v => v.ServerId).NotEmpty() means != 0 for int.
        var command = new UpdateServerCommand(
            ServerId: 0,
            Name: "Update Server",
            Ipaddress: "192.168.1.1",
            Status: ServerStatus.Up,
            Description: "Update Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ServerId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyName_ReturnsError(string name)
    {
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: name,
            Ipaddress: "192.168.1.1",
            Status: ServerStatus.Up,
            Description: "Update Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_LongName_ReturnsError()
    {
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: new string('a', 101),
            Ipaddress: "192.168.1.1",
            Status: ServerStatus.Up,
            Description: "Update Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_LongIpAddress_ReturnsError()
    {
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: new string('a', 51),
            Status: ServerStatus.Up,
            Description: "Update Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Ipaddress);
    }

    [Fact]
    public void Validate_InvalidStatus_ReturnsError()
    {
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Status: (ServerStatus)999,
            Description: "Update Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Validate_LongDescription_ReturnsError()
    {
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Status: ServerStatus.Up,
            Description: new string('a', 251)
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
