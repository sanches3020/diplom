using Sofia.Web.Models;

namespace Sofia.Web.Services;

public class ChatStorage
{
    private readonly Dictionary<string, List<ChatMessage>> _rooms = new();
    private const int MaxMessagesPerRoom = 50;

    public IReadOnlyList<ChatMessage> GetMessages(string room)
    {
        if (_rooms.TryGetValue(room, out var list))
            return list;

        return Array.Empty<ChatMessage>();
    }

    public void AddMessage(string room, ChatMessage message)
    {
        if (!_rooms.TryGetValue(room, out var list))
        {
            list = new List<ChatMessage>();
            _rooms[room] = list;
        }

        list.Add(message);

        if (list.Count > MaxMessagesPerRoom)
            list.RemoveAt(0);
    }
}
