using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Users.Commands;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Tests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace InfraMonitor.Tests.Unit.Application.Features.Users.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IBackgroundJobService> _mockBackgroundJob;
    private readonly TestDbContext _dbContext;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new TestDbContext(options);
        
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockBackgroundJob = new Mock<IBackgroundJobService>();

        _handler = new RegisterUserCommandHandler(
            _dbContext,
            _mockPasswordHasher.Object,
            _mockJwtTokenGenerator.Object,
            _mockBackgroundJob.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateFirstUserAsAdmin_WhenDatabaseIsEmpty()
    {
        // Arrange
        var command = new RegisterUserCommand("AdminUser", "admin@example.com", "AdminPass123");

        _mockPasswordHasher.Setup(p => p.HashPassword("AdminPass123"))
            .Returns("HashedAdminPass");

        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("admin-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be("Admin");
        result.Token.Should().Be("admin-token");

        var userInDb = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == "admin@example.com");
        userInDb.Should().NotBeNull();
        userInDb!.Role.Name.Should().Be("Admin");
        userInDb.PasswordHash.Should().Be("HashedAdminPass");

        // Verify background job scheduled
        _mockBackgroundJob.Verify(b => b.Schedule(
            It.IsAny<Expression<Func<IEmailService, Task>>>(), 
            It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCreateSubsequentUserAsUser_WhenDatabaseIsNotEmpty()
    {
        // Arrange
        // Add existing user (Admin)
        var existingRole = new Role { Name = "Admin" };
        var existingUser = new User { UserName = "Existing", Email = "existing@example.com", Role = existingRole, PasswordHash = "hash" };
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        var command = new RegisterUserCommand("NormalUser", "user@example.com", "UserPass123");

        _mockPasswordHasher.Setup(p => p.HashPassword("UserPass123"))
            .Returns("HashedUserPass");
            
        _mockJwtTokenGenerator.Setup(j => j.GenerateToken(It.IsAny<User>()))
            .Returns("user-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Role.Should().Be("User");
        
        // Verify User Role created if not exists
        var userRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
        userRole.Should().NotBeNull();
    }
}
