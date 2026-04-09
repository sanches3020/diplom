using Sofia.Web.Models;

namespace Sofia.Web.Services;

public interface IChatService
{
    IReadOnlyList<ChatMessage> GetHistory(string room);
    ChatMessage AddUserMessage(string room, string userId, string userName, string text);
    ChatMessage AddSystemMessage(string room, string text);
    bool IsUserBlocked(string userId);
}
