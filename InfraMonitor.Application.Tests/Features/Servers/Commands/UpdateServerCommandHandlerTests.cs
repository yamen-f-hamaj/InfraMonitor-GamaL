using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Servers.Commands;

public class UpdateServerCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly UpdateServerCommandHandler _handler;
    private readonly List<Server> _servers;

    public UpdateServerCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _servers = new List<Server>
        {
            new Server
            {
                ServerId = 1,
                Name = "Original Server",
                Ipaddress = "192.168.1.1",
                Description = "Original Description",
                Status = ServerStatus.Up,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_servers);
        _mockContext.Setup(c => c.Servers).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new UpdateServerCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesServerProperties()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Updated Server",
            Ipaddress: "192.168.1.200",
            Description: "Updated Description",
            Status: ServerStatus.Down
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var updatedServer = _servers.First();
        updatedServer.Name.Should().Be("Updated Server");
        updatedServer.Ipaddress.Should().Be("192.168.1.200");
        updatedServer.Description.Should().Be("Updated Description");
        updatedServer.Status.Should().Be(ServerStatus.Down);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChangesAsync()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Updated Server",
            Ipaddress: "192.168.1.200",
            Description: "Updated Description",
            Status: ServerStatus.Down
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentServer_ReturnsFalse()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 999,
            Name: "Updated Server",
            Ipaddress: "192.168.1.200",
            Description: "Updated Description",
            Status: ServerStatus.Down
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_UpdateWithNullValues_UpdatesSuccessfully()
    {
        // Arrange
        var command = new UpdateServerCommand(
            ServerId: 1,
            Name: "Server with Nulls",
            Ipaddress: null,
            Description: null,
            Status: ServerStatus.Maintenance
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var updatedServer = _servers.First();
        updatedServer.Name.Should().Be("Server with Nulls");
        updatedServer.Ipaddress.Should().BeNull();
        updatedServer.Description.Should().BeNull();
        updatedServer.Status.Should().Be(ServerStatus.Maintenance);
    }
}
