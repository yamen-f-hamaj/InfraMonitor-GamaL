using Microsoft.AspNetCore.SignalR;

namespace InfraMonitor.WebAPI.Hubs;

public class MetricsHub : Hub
{
    public async Task JoinServerGroup(string serverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Server_{serverId}");
    }

    public async Task LeaveServerGroup(string serverId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Server_{serverId}");
    }
}
