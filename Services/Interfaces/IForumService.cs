using Sofia.Web.Models;

public interface IForumService
{
    Task<List<ForumCategory>> GetCategoriesAsync();
    Task<List<ForumThread>> GetThreadsByCategoryAsync(int categoryId);
    Task<ForumThread?> GetThreadAsync(int threadId);

    Task CreateThreadAsync(int categoryId, string title, string authorId);
    Task CreatePostAsync(int threadId, string content, string authorId);
    Task CreatePostAsync(int threadId, string content, string authorId, Guid? mediaFileId = null);

    Task ToggleLikeAsync(int postId, string userId);
}
