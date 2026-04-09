using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/forum")]
public class ForumController : ControllerBase
{
    private readonly IForumService _forumService;

    public ForumController(IForumService forumService)
    {
        _forumService = forumService;
    }

    // ---------------------------------------------------------
    // 1. Получить список категорий
    // ---------------------------------------------------------
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _forumService.GetCategoriesAsync();

        var result = categories.Select(c => new ForumCategoryDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            ThreadsCount = c.Threads.Count
        });

        return Ok(result);
    }

    // ---------------------------------------------------------
    // 2. Получить темы категории
    // ---------------------------------------------------------
    [HttpGet("categories/{categoryId}/threads")]
    public async Task<IActionResult> GetThreads(int categoryId)
    {
        var threads = await _forumService.GetThreadsByCategoryAsync(categoryId);

        var result = threads.Select(t => new ForumThreadDto
        {
            Id = t.Id,
            Title = t.Title,
            CreatedAt = t.CreatedAt,
            PostsCount = t.Posts.Count
        });

        return Ok(result);
    }

    // ---------------------------------------------------------
    // 3. Создать тему (только авторизованные)
    // ---------------------------------------------------------
    [Authorize]
    [HttpPost("threads")]
    public async Task<IActionResult> CreateThread(CreateThreadDto dto)
    {
        var userId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        await _forumService.CreateThreadAsync(dto.CategoryId, dto.Title, userId);

        return Ok(new { message = "Thread created" });
    }

    // ---------------------------------------------------------
    // 4. Получить тему + посты + медиа
    // ---------------------------------------------------------
    [HttpGet("threads/{threadId}")]
    public async Task<IActionResult> GetThread(int threadId)
    {
        var thread = await _forumService.GetThreadAsync(threadId);
        if (thread == null)
            return NotFound();

        var result = new ForumThreadFullDto
        {
            Id = thread.Id,
            Title = thread.Title,
            CreatedAt = thread.CreatedAt,

            Posts = thread.Posts.Select(p => new ForumPostDto
            {
                Id = p.Id,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                Likes = p.Likes.Count,
                AuthorId = p.AuthorId,

                // 🔥 Добавлено: URL медиа-файла
                MediaUrl = p.MediaFile != null
                    ? "/" + p.MediaFile.FilePath
                    : null
            }).ToList()
        };

        return Ok(result);
    }

    // ---------------------------------------------------------
    // 5. Создать пост (только авторизованные)
    // ---------------------------------------------------------
    [Authorize]
    [HttpPost("posts")]
    public async Task<IActionResult> CreatePost(CreatePostDto dto)
    {
        var userId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        await _forumService.CreatePostAsync(dto.ThreadId, dto.Content, userId, dto.MediaFileId);

        return Ok(new { message = "Post created" });
    }

    // ---------------------------------------------------------
    // 6. Лайк / дизлайк поста (только авторизованные)
    // ---------------------------------------------------------
    [Authorize]
    [HttpPost("posts/{postId}/like")]
    public async Task<IActionResult> ToggleLike(int postId)
    {
        var userId = User.FindFirst("sub")?.Value
                     ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized();

        await _forumService.ToggleLikeAsync(postId, userId);

        return Ok(new { message = "Like toggled" });
    }
}
