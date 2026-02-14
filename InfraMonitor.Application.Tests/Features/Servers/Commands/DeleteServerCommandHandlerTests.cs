using FluentAssertions;
using InfraMonitor.Application.Common.Interfaces;
using InfraMonitor.Application.Features.Servers.Commands;
using InfraMonitor.Application.Tests.Helpers;
using InfraMonitor.Domain.Entities;
using InfraMonitor.Domain.Enums;
using Moq;

namespace InfraMonitor.Application.Tests.Features.Servers.Commands;

public class DeleteServerCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly DeleteServerCommandHandler _handler;
    private readonly List<Server> _servers;

    public DeleteServerCommandHandlerTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _servers = new List<Server>
        {
            new Server
            {
                ServerId = 1,
                Name = "Server to Delete",
                Ipaddress = "192.168.1.1",
                Description = "Will be deleted",
                Status = ServerStatus.Up,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Server
            {
                ServerId = 2,
                Name = "Another Server",
                Ipaddress = "192.168.1.2",
                Description = "Should remain",
                Status = ServerStatus.Up,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        var mockDbSet = MockDbSetHelper.CreateMockDbSetWithAsyncQueryProvider(_servers);
        _mockContext.Setup(c => c.Servers).Returns(mockDbSet.Object);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _handler = new DeleteServerCommandHandler(_mockContext.Object);
    }

    [Fact]
    public async Task Handle_ValidServerId_DeletesServerSuccessfully()
    {
        // Arrange
        var command = new DeleteServerCommand(ServerId: 1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _servers.Should().HaveCount(1);
        _servers.Should().NotContain(s => s.ServerId == 1);
        _servers.Should().Contain(s => s.ServerId == 2);
    }

    [Fact]
    public async Task Handle_ValidServerId_CallsSaveChangesAsync()
    {
        // Arrange
        var command = new DeleteServerCommand(ServerId: 1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentServer_ReturnsFalse()
    {
        // Arrange
        var command = new DeleteServerCommand(ServerId: 999);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _servers.Should().HaveCount(2); // No servers deleted
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidServerId_CallsRemoveOnDbSet()
    {
        // Arrange
        var command = new DeleteServerCommand(ServerId: 1);
        var mockDbSet = _mockContext.Object.Servers;

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Mock.Get(mockDbSet).Verify(m => m.Remove(It.IsAny<Server>()), Times.Once);
    }
}
