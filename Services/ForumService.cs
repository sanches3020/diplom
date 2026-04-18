using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.Models;

namespace Sofia.Web.Services;

public class ForumService : IForumService
{
    private readonly SofiaDbContext _db;

    public ForumService(SofiaDbContext db)
    {
        _db = db;
    }

    public async Task<List<ForumCategory>> GetCategoriesAsync()
    {
        return await _db.ForumCategories
            .Include(c => c.Threads)
            .ToListAsync();
    }

    public async Task<List<ForumThread>> GetThreadsByCategoryAsync(int categoryId)
    {
        return await _db.ForumThreads
            .Where(t => t.CategoryId == categoryId)
            .Include(t => t.Posts)
            .ToListAsync();
    }

    public async Task<ForumThread?> GetThreadAsync(int threadId)
    {
        return await _db.ForumThreads
            .Include(t => t.Posts)
                .ThenInclude(p => p.Likes)
            .Include(t => t.Posts)
                .ThenInclude(p => p.MediaFile)
            .FirstOrDefaultAsync(t => t.Id == threadId);
    }

    public async Task CreateThreadAsync(int categoryId, string title, string authorId)
    {
        var thread = new ForumThread
        {
            CategoryId = categoryId,
            Title = title,
            CreatedAt = DateTime.UtcNow,
            AuthorId = authorId
        };

        _db.ForumThreads.Add(thread);
        await _db.SaveChangesAsync();
    }

    public async Task CreatePostAsync(int threadId, string content, string authorId, Guid? mediaFileId = null)
    {
        var post = new ForumPost
        {
            ThreadId = threadId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            AuthorId = authorId,
            MediaFileId = mediaFileId
        };

        _db.ForumPosts.Add(post);
        await _db.SaveChangesAsync();
    }

    public async Task ToggleLikeAsync(int postId, string userId)
    {
        var existing = await _db.ForumPostLikes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existing != null)
        {
            _db.ForumPostLikes.Remove(existing);
        }
        else
        {
            _db.ForumPostLikes.Add(new ForumPostLike
            {
                PostId = postId,
                UserId = userId
            });
        }

        await _db.SaveChangesAsync();
    }

    public Task CreatePostAsync(int threadId, string content, string authorId)
    {
        throw new NotImplementedException();
    }
}
