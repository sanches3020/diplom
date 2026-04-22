using Microsoft.AspNetCore.SignalR;
using Sofia.Web.Services;
using System.Collections.Concurrent;

namespace Sofia.Web.Hubs;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private static readonly ConcurrentDictionary<string, string> ConnectionRooms = new();

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    // Подключение к комнате
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task JoinRoom(string room)
    {
        room = string.IsNullOrWhiteSpace(room) ? "general" : room.Trim();
        if (ConnectionRooms.TryGetValue(Context.ConnectionId, out var previousRoom) && previousRoom != room)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, previousRoom);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, room);
        ConnectionRooms[Context.ConnectionId] = room;

        var userId = GetUserId() ?? "guest";
        var userName = GetUserName();

        if (_chatService.IsUserBlocked(userId))
        {
            await Clients.Caller.SendAsync("Blocked", "Вы заблокированы в чате.");
            return;
        }

        // отправляем историю только этому клиенту
        var history = _chatService.GetHistory(room);
        await Clients.Caller.SendAsync("ReceiveHistory", room, history);

        // системное сообщение о входе
        var sys = _chatService.AddSystemMessage(room, $"{userName} вошёл в комнату.");
        await Clients.Group(room).SendAsync("ReceiveMessage", room, sys);
    }

    public async Task LeaveRoom(string room)
    {
        room = string.IsNullOrWhiteSpace(room) ? "general" : room.Trim();
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
        ConnectionRooms.TryRemove(Context.ConnectionId, out _);

        var userName = GetUserName();
        var sys = _chatService.AddSystemMessage(room, $"{userName} покинул комнату.");
        await Clients.Group(room).SendAsync("ReceiveMessage", room, sys);
    }

    public async Task SendMessage(string room, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        room = string.IsNullOrWhiteSpace(room) ? "general" : room.Trim();
        var userId = GetUserId() ?? "guest";
        var userName = GetUserName();

        if (_chatService.IsUserBlocked(userId))
        {
            await Clients.Caller.SendAsync("Blocked", "Вы заблокированы в чате.");
            return;
        }

        var msg = _chatService.AddUserMessage(room, userId, userName, text.Trim());

        await Clients.Group(room).SendAsync("ReceiveMessage", room, msg);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (ConnectionRooms.TryRemove(Context.ConnectionId, out var room))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, room);
            var userName = GetUserName();
            var sys = _chatService.AddSystemMessage(room, $"{userName} отключился.");
            await Clients.Group(room).SendAsync("ReceiveMessage", room, sys);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private string? GetUserId()
    {
        return Context.User?.FindFirst("sub")?.Value
               ?? Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }

    private string GetUserName()
    {
        if (Context.User?.Identity?.IsAuthenticated == true)
            return Context.User.Identity.Name ?? "Пользователь";

        return $"Гость-{Random.Shared.Next(1000, 9999)}";
    }
}
