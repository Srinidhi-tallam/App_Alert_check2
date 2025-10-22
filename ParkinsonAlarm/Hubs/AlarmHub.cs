using Microsoft.AspNetCore.SignalR;

namespace ParkinsonAlarm.Hubs;

public class AlarmHub : Hub
{
    public Task JoinGroup(string groupName)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public Task LeaveGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public Task ClientAck(Guid alarmId)
    {
        Console.WriteLine($"Client {Context.ConnectionId} acknowledged alarm {alarmId}");
        return Task.CompletedTask;
    }
}