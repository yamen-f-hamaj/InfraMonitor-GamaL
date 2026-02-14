using FluentValidation.TestHelper;
using InfraMonitor.Application.Features.Servers.Commands;
using Xunit;

namespace InfraMonitor.Application.Tests.Features.Servers.Commands;

public class CreateServerCommandValidatorTests
{
    private readonly CreateServerCommandValidator _validator;

    public CreateServerCommandValidatorTests()
    {
        _validator = new CreateServerCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: "Test Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyName_ReturnsError(string name)
    {
        var command = new CreateServerCommand(
            Name: name,
            Ipaddress: "192.168.1.1",
            Description: "Test Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_LongName_ReturnsError()
    {
        var command = new CreateServerCommand(
            Name: new string('a', 101),
            Ipaddress: "192.168.1.1",
            Description: "Test Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData("invalid-ip")]
    [InlineData("256.256.256.256")]
    [InlineData("1.1.1")]
    public void Validate_InvalidIpAddress_ReturnsError(string ip)
    {
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: ip,
            Description: "Test Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Ipaddress);
    }

    [Fact]
    public void Validate_LongDescription_ReturnsError()
    {
        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "192.168.1.1",
            Description: new string('a', 251)
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Validate_EmptyIpAddress_IsAllowed_IfOptional()
    {
        // Assuming IP might be optional based on current rules -> wait, the rule says When(v => !string.IsNullOrEmpty(v.Ipaddress))
        // So empty/null should be valid if not explicitly marked NotEmpty.
        // RuleFor(v => v.Ipaddress).MaximumLength(50).Matches(...).When(...)
        // It does NOT have NotEmpty().

        var command = new CreateServerCommand(
            Name: "Test Server",
            Ipaddress: "",
            Description: "Test Description"
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Ipaddress);
    }
}
