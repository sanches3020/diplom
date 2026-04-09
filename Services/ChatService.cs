using Sofia.Web.Data;
using Sofia.Web.Models;

namespace Sofia.Web.Services;

public class ChatService : IChatService
{
    private readonly ChatStorage _storage;
    private readonly SofiaDbContext _db;

    // простейший список запрещённых слов
    private static readonly string[] BannedWords =
        ["дурак", "идиот", "сука"]; // сюда можно добавить свои

    public ChatService(ChatStorage storage, SofiaDbContext db)
    {
        _storage = storage;
        _db = db;
    }

    public IReadOnlyList<ChatMessage> GetHistory(string room) =>
        _storage.GetMessages(room);

    public bool IsUserBlocked(string userId)
    {
        var user = _db.Users.FirstOrDefault(u => u.Id == userId);
        return user?.IsBlocked == true;
    }

    public ChatMessage AddUserMessage(string room, string userId, string userName, string text)
    {
        // модерация: бан слов
        if (ContainsBannedWords(text))
            text = "[сообщение скрыто модерацией]";

        var msg = new ChatMessage
        {
            Room = room,
            UserId = userId,
            UserName = userName,
            AvatarCode = GenerateAvatarCode(userId, userName),
            Text = text,
            IsSystem = false
        };

        _storage.AddMessage(room, msg);
        return msg;
    }

    public ChatMessage AddSystemMessage(string room, string text)
    {
        var msg = new ChatMessage
        {
            Room = room,
            UserId = "system",
            UserName = "Система",
            AvatarCode = "system",
            Text = text,
            IsSystem = true
        };

        _storage.AddMessage(room, msg);
        return msg;
    }

    private static bool ContainsBannedWords(string text)
    {
        var lower = text.ToLowerInvariant();
        return BannedWords.Any(w => lower.Contains(w));
    }

    private static string GenerateAvatarCode(string userId, string userName)
    {
        // простая анонимная аватарка: цвет + инициалы
        var colors = new[] { "purple", "blue", "green", "orange", "pink" };
        var color = colors[Math.Abs(userId.GetHashCode()) % colors.Length];

        var initials = new string(userName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => char.ToUpperInvariant(p[0]))
            .Take(2)
            .ToArray());

        return $"{color}:{initials}";
    }
}
