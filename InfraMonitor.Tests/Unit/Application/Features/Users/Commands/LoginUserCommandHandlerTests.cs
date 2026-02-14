using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using InfraMonitor.Application.Common.Exceptions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Users.Commands;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Tests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace InfraMonitor.Tests.Unit.Application.Features.Users.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly TestDbContext _dbContext;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        // Setup TestDbContext with InMemory provider
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new TestDbContext(options);
        
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        _handler = new LoginUserCommandHandler(
            _dbContext,
            _mockPasswordHasher.Object,
            _mockJwtTokenGenerator.Object);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var role = new Role { RoleId = 1, Name = "Admin" };
        var user = new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@example.com",
            PasswordHash = "HashedPassword",
            RoleId = 1,
            Role = role
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var command = new LoginUserCommand("test@example.com", "MyPassword");

        _mockPasswordHasher.Setup(p => p.VerifyPassword("MyPassword", "HashedPassword"))
            .Returns(true);

        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("valid-jwt-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("valid-jwt-token");
        result.Email.Should().Be("test@example.com");
        result.Role.Should().Be("Admin");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedException_WhenUserDoesNotExist()
    {
        // Arrange
        // No user in DB
        var command = new LoginUserCommand("nonexistent@example.com", "password");

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials.");
    }

    [Fact]
    public async Task Handle_ShouldThrowUnauthorizedException_WhenPasswordIsInvalid()
    {
        // Arrange
        var role = new Role { RoleId = 1, Name = "Admin" };
        var user = new User
        {
            UserId = 1,
            UserName = "TestUser",
            Email = "test@example.com",
            PasswordHash = "HashedPassword",
            Role = role
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var command = new LoginUserCommand("test@example.com", "WrongPassword");

        _mockPasswordHasher.Setup(p => p.VerifyPassword("WrongPassword", "HashedPassword"))
            .Returns(false);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid credentials.");
    }
}
