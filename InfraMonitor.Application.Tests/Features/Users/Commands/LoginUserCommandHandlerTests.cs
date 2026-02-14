using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Users.Commands;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Users.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly LoginUserCommandHandler _handler;
    private readonly List<User> _users;

    public LoginUserCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        _users = new List<User>
        {
            new User
            {
                UserId = 1,
                UserName = "testuser",
                Email = "test@test.com",
                PasswordHash = "hashed_password",
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                Role = new Role
                {
                    RoleId = 1,
                    Name = "User",
                    Description = "Standard User"
                }
            }
        };

        var mockUserDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_users);
        _mockContext.Setup(c => c.Users).Returns(mockUserDbSet.Object);

        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("test_jwt_token");

        _handler = new LoginUserCommandHandler(
            _mockContext.Object,
            _mockPasswordHasher.Object,
            _mockJwtTokenGenerator.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: "password123"
        );

        _mockPasswordHasher.Setup(p => p.VerifyPassword("password123", "hashed_password"))
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<AuthResponse>();
        result.UserId.Should().Be(1);
        result.UserName.Should().Be("testuser");
        result.Email.Should().Be("test@test.com");
        result.Role.Should().Be("User");
        result.Token.Should().Be("test_jwt_token");
    }

    [Fact]
    public async Task Handle_ValidCredentials_VerifiesPassword()
    {
        // Arrange
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: "password123"
        );

        _mockPasswordHasher.Setup(p => p.VerifyPassword("password123", "hashed_password"))
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockPasswordHasher.Verify(
            p => p.VerifyPassword("password123", "hashed_password"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCredentials_GeneratesJwtToken()
    {
        // Arrange
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: "password123"
        );

        _mockPasswordHasher.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockJwtTokenGenerator.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsException()
    {
        // Arrange
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: "wrongpassword"
        );

        _mockPasswordHasher.Setup(p => p.VerifyPassword("wrongpassword", "hashed_password"))
            .Returns(false);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_NonExistentEmail_ThrowsException()
    {
        // Arrange
        var command = new LoginUserCommand(
            Email: "nonexistent@test.com",
            Password: "password123"
        );

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_InvalidCredentials_DoesNotGenerateToken()
    {
        // Arrange
        var command = new LoginUserCommand(
            Email: "test@test.com",
            Password: "wrongpassword"
        );

        _mockPasswordHasher.Setup(p => p.VerifyPassword("wrongpassword", "hashed_password"))
            .Returns(false);

        // Act & Assert
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch
        {
            // Expected exception
        }

        _mockJwtTokenGenerator.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Never);
    }
}
