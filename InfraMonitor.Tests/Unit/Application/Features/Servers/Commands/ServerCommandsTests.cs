using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using InfraMonitor.Tests.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace InfraMonitor.Tests.Unit.Application.Features.Servers.Commands;

public class ServerCommandsTests
{
    private readonly TestDbContext _dbContext;

    public ServerCommandsTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new TestDbContext(options);
    }

    [Fact]
    public async Task CreateServer_ShouldAddServerToDatabase()
    {
        // Arrange
        var handler = new CreateServerCommandHandler(_dbContext);
        var command = new CreateServerCommand("Production DB", "192.168.1.10", "Main SQL Server");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);
        
        var server = await _dbContext.Servers.FindAsync(result);
        server.Should().NotBeNull();
        server!.Name.Should().Be("Production DB");
        server.Ipaddress.Should().Be("192.168.1.10");
        server.Status.Should().Be(ServerStatus.Up);
    }

    [Fact]
    public async Task UpdateServer_ShouldUpdateExistingServer()
    {
        // Arrange
        var server = new Server { Name = "Old Name", Ipaddress = "10.0.0.1", Status = ServerStatus.Down };
        await _dbContext.Servers.AddAsync(server);
        await _dbContext.SaveChangesAsync();

        var handler = new UpdateServerCommandHandler(_dbContext);
        var command = new UpdateServerCommand(server.ServerId, "New Name", "10.0.0.2", "Updated Desc", ServerStatus.Up);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        var updatedServer = await _dbContext.Servers.FindAsync(server.ServerId);
        updatedServer!.Name.Should().Be("New Name");
        updatedServer.Ipaddress.Should().Be("10.0.0.2");
        updatedServer.Status.Should().Be(ServerStatus.Up);
    }

    [Fact]
    public async Task UpdateServer_ShouldReturnFalse_WhenServerDoesNotExist()
    {
        // Arrange
        var handler = new UpdateServerCommandHandler(_dbContext);
        var command = new UpdateServerCommand(999, "New Name", "10.0.0.2", "Updated Desc", ServerStatus.Up);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteServer_ShouldRemoveServerFromDatabase()
    {
        // Arrange
        var server = new Server { Name = "To Delete", Ipaddress = "1.1.1.1" };
        await _dbContext.Servers.AddAsync(server);
        await _dbContext.SaveChangesAsync();

        var handler = new DeleteServerCommandHandler(_dbContext);
        var command = new DeleteServerCommand(server.ServerId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        
        var deletedServer = await _dbContext.Servers.FindAsync(server.ServerId);
        deletedServer.Should().BeNull();
    }

    [Fact]
    public async Task DeleteServer_ShouldReturnFalse_WhenServerDoesNotExist()
    {
        // Arrange
        var handler = new DeleteServerCommandHandler(_dbContext);
        var command = new DeleteServerCommand(999);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}
