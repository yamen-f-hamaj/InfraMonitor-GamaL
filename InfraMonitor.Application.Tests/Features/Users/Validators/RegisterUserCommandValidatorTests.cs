using FluentValidation.TestHelper;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Users.Commands;
using InfraMonitor.Application.Features.Users.Validators;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using Moq;
using Xunit;

namespace InfraMonitor.Application.Tests.Features.Users.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly RegisterUserCommandValidator _validator;
    private readonly List<User> _users;

    public RegisterUserCommandValidatorTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _users = new List<User>
        {
            new User { UserName = "existingUser", Email = "existing@test.com" }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_users);
        _mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

        _validator = new RegisterUserCommandValidator(_mockContext.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ReturnsTrue()
    {
        var command = new RegisterUserCommand(
            UserName: "newUser",
            Email: "new@test.com",
            Password: "password123"
        );

        var result = await _validator.TestValidateAsync(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_EmptyUserName_ReturnsError(string userName)
    {
        var command = new RegisterUserCommand(
            UserName: userName,
            Email: "new@test.com",
            Password: "password123"
        );

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public async Task Validate_DuplicateUserName_ReturnsError()
    {
        var command = new RegisterUserCommand(
            UserName: "existingUser",
            Email: "new@test.com",
            Password: "password123"
        );

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.UserName)
              .WithErrorMessage("The specified username is already taken.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_EmptyEmail_ReturnsError(string email)
    {
        var command = new RegisterUserCommand(
            UserName: "newUser",
            Email: email,
            Password: "password123"
        );

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Validate_DuplicateEmail_ReturnsError()
    {
        var command = new RegisterUserCommand(
            UserName: "newUser",
            Email: "existing@test.com",
            Password: "password123"
        );

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("The specified email is already taken.");
    }

    [Fact]
    public async Task Validate_ShortPassword_ReturnsError()
    {
        var command = new RegisterUserCommand(
            UserName: "newUser",
            Email: "new@test.com",
            Password: "123" // Too short
        );

        var result = await _validator.TestValidateAsync(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
