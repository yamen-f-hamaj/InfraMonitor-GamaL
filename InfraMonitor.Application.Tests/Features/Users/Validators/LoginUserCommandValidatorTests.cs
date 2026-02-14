using FluentValidation.TestHelper;
using InfraMonitor.Application.Features.Users.Commands;
using InfraMonitor.Application.Features.Users.Validators;
using Xunit;

namespace InfraMonitor.Application.Tests.Features.Users.Validators;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator;

    public LoginUserCommandValidatorTests()
    {
        _validator = new LoginUserCommandValidator();
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsTrue()
    {
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: "password123"
        );

        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyEmail_ReturnsError(string email)
    {
        var command = new LoginUserCommand(
            Email: email,
            Password: "password123"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_InvalidEmail_ReturnsError()
    {
        var command = new LoginUserCommand(
            Email: "invalid-email",
            Password: "password123"
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyPassword_ReturnsError(string password)
    {
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: password
        );

        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
