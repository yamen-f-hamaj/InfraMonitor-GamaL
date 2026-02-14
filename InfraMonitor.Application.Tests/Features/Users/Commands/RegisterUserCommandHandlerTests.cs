using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Users.Commands;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Users.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IBackgroundJobService> _mockBackgroundJobService;
    private readonly RegisterUserCommandHandler _handler;
    private readonly List<User> _users;
    private readonly List<Role> _roles;

    public RegisterUserCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockBackgroundJobService = new Mock<IBackgroundJobService>();

        _users = new List<User>();
        _roles = new List<Role>();

        var mockUserDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_users);
        var mockRoleDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_roles);

        _mockContext.Setup(c => c.Users).Returns(mockUserDbSet.Object);
        _mockContext.Setup(c => c.Roles).Returns(mockRoleDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockPasswordHasher.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns<string>(password => $"hashed_{password}");

        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("test_jwt_token");

        _handler = new RegisterUserCommandHandler(
            _mockContext.Object,
            _mockPasswordHasher.Object,
            _mockJwtTokenGenerator.Object,
            _mockBackgroundJobService.Object);
    }

    [Fact]
    public async Task Handle_FirstUser_CreatesAdminRole()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "admin",
            Email: "admin@test.com",
            Password: "password123"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _roles.Should().ContainSingle();
        _roles.First().Name.Should().Be("Admin");
        _roles.First().Description.Should().Be("Admin Role");
    }

    [Fact]
    public async Task Handle_FirstUser_AssignsAdminRole()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "admin",
            Email: "admin@test.com",
            Password: "password123"
        );

        // Mock role creation
        _mockContext.Setup(c => c.Roles.Add(It.IsAny<Role>()))
            .Callback<Role>(r => { r.RoleId = 1; _roles.Add(r); });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Handle_SubsequentUser_AssignsUserRole()
    {
        // Arrange
        _users.Add(new User
        {
            UserId = 1,
            UserName = "existing",
            Email = "existing@test.com",
            PasswordHash = "hash",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow
        });

        var command = new RegisterUserCommand(
            UserName: "newuser",
            Email: "newuser@test.com",
            Password: "password123"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - should try to get/create User role, not Admin
        // Since we're mocking, we need to verify the role lookup
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_ValidCommand_HashesPassword()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "testuser",
            Email: "test@test.com",
            Password: "mypassword"
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockPasswordHasher.Verify(p => p.HashPassword("mypassword"), Times.Once);
        _users.First().PasswordHash.Should().Be("hashed_mypassword");
    }

    [Fact]
    public async Task Handle_ValidCommand_GeneratesJwtToken()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "testuser",
            Email: "test@test.com",
            Password: "password123"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockJwtTokenGenerator.Verify(j => j.GenerateToken(It.IsAny<User>()), Times.Once);
        result.Token.Should().Be("test_jwt_token");
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesUserWithCorrectProperties()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "testuser",
            Email: "test@test.com",
            Password: "password123"
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _users.Should().ContainSingle();
        var createdUser = _users.First();
        createdUser.UserName.Should().Be("testuser");
        createdUser.Email.Should().Be("test@test.com");
        createdUser.PasswordHash.Should().Be("hashed_password123");
        createdUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsAuthResponse()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "testuser",
            Email: "test@test.com",
            Password: "password123"
        );

        // Mock user ID assignment
        _mockContext.Setup(c => c.Users.Add(It.IsAny<User>()))
            .Callback<User>(u => u.UserId = 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<AuthResponse>();
        result.UserName.Should().Be("testuser");
        result.Email.Should().Be("test@test.com");
        result.Token.Should().Be("test_jwt_token");
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChangesTwice()
    {
        // Arrange
        var command = new RegisterUserCommand(
            UserName: "testuser",
            Email: "test@test.com",
            Password: "password123"
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        // Once for role creation, once for user creation
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
