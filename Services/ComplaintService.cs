using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Complaints;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;

namespace Sofia.Web.Services;

public class ComplaintService : IComplaintService
{
    private readonly SofiaDbContext _context;
    private readonly INotificationsService _notificationsService;
    private readonly IAdminService _adminService;

    public ComplaintService(
        SofiaDbContext context,
        INotificationsService notificationsService,
        IAdminService adminService)
    {
        _context = context;
        _notificationsService = notificationsService;
        _adminService = adminService;
    }

    /// <summary>
    /// Создать жалобу
    /// </summary>
    public async Task<ComplaintDto?> CreateComplaintAsync(string senderUserId, CreateComplaintRequest request)
    {
        // Проверка валидации
        var validation = await CanComplainAsync(senderUserId, request.TargetUserId, request.MessageId, request.PostId);
        if (!validation.canComplain)
        {
            return null;
        }

        // Проверка - не спамит ли пользователь жалобами
        var recentComplaints = await _context.Complaints
            .Where(c => c.SenderUserId == senderUserId && c.CreatedAt > DateTime.UtcNow.AddHours(-1))
            .CountAsync();

        if (recentComplaints >= 5)
        {
            return null; // Спам - слишком много жалоб за час
        }

        // Создаем жалобу
        var complaint = new Complaint
        {
            SenderUserId = senderUserId,
            TargetUserId = request.TargetUserId,
            MessageId = request.MessageId,
            PostId = request.PostId,
            Reason = (ComplaintReason)request.Reason,
            ReasonText = request.ReasonText,
            Status = ComplaintStatus.New,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Complaints.Add(complaint);
        await _context.SaveChangesAsync();

        // Отправляем уведомление администраторам о новой жалобе
        await SendAdminNotificationAsync(complaint, "new_complaint");

        return await GetComplaintByIdAsync(complaint.Id);
    }

    /// <summary>
    /// Получить все жалобы (только для администраторов)
    /// </summary>
    public async Task<List<ComplaintDto>> GetAllComplaintsAsync(int? statusFilter = null)
    {
        var query = _context.Complaints
            .AsNoTracking()
            .Include(c => c.TargetUser)
            .Include(c => c.Message)
            .Include(c => c.Post)
            .AsQueryable();

        if (statusFilter.HasValue)
        {
            query = query.Where(c => (int)c.Status == statusFilter.Value);
        }

        var complaints = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return complaints.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Получить жалобы на конкретного пользователя
    /// </summary>
    public async Task<List<ComplaintDto>> GetComplaintsOnUserAsync(string targetUserId)
    {
        var complaints = await _context.Complaints
            .AsNoTracking()
            .Where(c => c.TargetUserId == targetUserId)
            .Include(c => c.TargetUser)
            .Include(c => c.Message)
            .Include(c => c.Post)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return complaints.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Получить жалобу по ID
    /// </summary>
    public async Task<ComplaintDto?> GetComplaintByIdAsync(int complaintId)
    {
        var complaint = await _context.Complaints
            .AsNoTracking()
            .Include(c => c.TargetUser)
            .Include(c => c.Message)
            .Include(c => c.Post)
            .Include(c => c.ReviewedByAdmin)
            .FirstOrDefaultAsync(c => c.Id == complaintId);

        if (complaint == null) return null;

        return MapToDto(complaint);
    }

    /// <summary>
    /// Обновить статус жалобы
    /// </summary>
    public async Task<bool> UpdateComplaintStatusAsync(int complaintId, string adminId, UpdateComplaintStatusRequest request)
    {
        var complaint = await _context.Complaints.FindAsync(complaintId);
        if (complaint == null) return false;

        var previousStatus = complaint.Status;
        complaint.Status = (ComplaintStatus)request.Status;
        complaint.ReviewedByAdminId = adminId;
        complaint.ReviewedAt = DateTime.UtcNow;
        complaint.AdminComment = request.AdminComment;
        complaint.UpdatedAt = DateTime.UtcNow;

        // Применять санкции
        if (request.ApplyBan && !complaint.IsBanned)
        {
            complaint.IsBanned = true;
            await _adminService.BlockUserAsync(complaint.TargetUserId);
            await _adminService.LogAsync(adminId, "BlockUserFromComplaint", complaint.TargetUserId, $"Complaint #{complaintId}");
        }

        if (request.ApplyWarning && !complaint.IsWarned)
        {
            complaint.IsWarned = true;
            // Отправляем уведомление пользователю о предупреждении
            await SendUserWarningNotificationAsync(complaint.TargetUserId);
        }

        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();

        // Отправляем уведомление об изменении статуса
        await SendStatusChangeNotificationAsync(complaint, previousStatus);

        return true;
    }

    /// <summary>
    /// Получить количество незавершенных жалоб
    /// </summary>
    public async Task<int> GetUnreviewedCountAsync()
    {
        return await _context.Complaints
            .CountAsync(c => c.Status == ComplaintStatus.New);
    }

    /// <summary>
    /// Получить статистику по жалобам
    /// </summary>
    public async Task<ComplaintStatsDto> GetComplaintStatsAsync()
    {
        var totalComplaints = await _context.Complaints.CountAsync();
        var newComplaints = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.New);
        var reviewedComplaints = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Reviewed);
        var resolvedComplaints = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Resolved);
        var dismissedComplaints = await _context.Complaints.CountAsync(c => c.Status == ComplaintStatus.Dismissed);

        var usersWithComplaints = await _context.Complaints
            .Select(c => c.TargetUserId)
            .Distinct()
            .CountAsync();

        var usersWithMultipleComplaints = await _context.Complaints
            .GroupBy(c => c.TargetUserId)
            .Where(g => g.Count() > 1)
            .CountAsync();

        return new ComplaintStatsDto
        {
            TotalComplaints = totalComplaints,
            NewComplaints = newComplaints,
            ReviewedComplaints = reviewedComplaints,
            ResolvedComplaints = resolvedComplaints,
            DismissedComplaints = dismissedComplaints,
            UsersWithComplaints = usersWithComplaints,
            UsersWithMultipleComplaints = usersWithMultipleComplaints
        };
    }

    /// <summary>
    /// Проверить, может ли пользователь подать жалобу на этот контент
    /// </summary>
    public async Task<(bool canComplain, string? reason)> CanComplainAsync(string senderUserId, string targetUserId, string? messageId = null, int? postId = null)
    {
        // Нельзя жаловаться на себя
        if (senderUserId == targetUserId)
        {
            return (false, "Cannot complain about yourself");
        }

        // Если есть MessageId, проверяем, что сообщение существует
        if (!string.IsNullOrEmpty(messageId))
        {
            var message = await _context.Set<ChatMessage>().FindAsync(messageId);
            if (message == null)
            {
                return (false, "Message not found");
            }

            if (message.UserId != targetUserId)
            {
                return (false, "Message does not belong to target user");
            }

            // Проверяем, есть ли такая жалоба
            var existingComplaint = await _context.Complaints
                .Where(c => c.SenderUserId == senderUserId && c.MessageId == messageId)
                .FirstOrDefaultAsync();

            if (existingComplaint != null)
            {
                return (false, "You have already complained about this message");
            }
        }

        // Если есть PostId, проверяем, что пост существует
        if (postId.HasValue)
        {
            var post = await _context.Set<ForumPost>().FindAsync(postId.Value);
            if (post == null)
            {
                return (false, "Post not found");
            }

            if (post.AuthorId != targetUserId)
            {
                return (false, "Post does not belong to target user");
            }

            // Проверяем, есть ли такая жалоба
            var existingComplaint = await _context.Complaints
                .Where(c => c.SenderUserId == senderUserId && c.PostId == postId.Value)
                .FirstOrDefaultAsync();

            if (existingComplaint != null)
            {
                return (false, "You have already complained about this post");
            }
        }

        // Если ни MessageId, ни PostId не указаны, то это жалоба на пользователя
        if (string.IsNullOrEmpty(messageId) && !postId.HasValue)
        {
            // Проверяем существование пользователя
            var targetUser = await _context.Users.FindAsync(targetUserId);
            if (targetUser == null)
            {
                return (false, "Target user not found");
            }

            // Проверяем, не жаловался ли уже на этого пользователя
            var existingComplaint = await _context.Complaints
                .Where(c => c.SenderUserId == senderUserId && c.TargetUserId == targetUserId && c.MessageId == null && c.PostId == null)
                .FirstOrDefaultAsync();

            if (existingComplaint != null)
            {
                return (false, "You have already complained about this user");
            }
        }

        return (true, null);
    }

    // ======================= Helper Methods =======================

    private ComplaintDto MapToDto(Complaint complaint)
    {
        return new ComplaintDto
        {
            Id = complaint.Id,
            SenderUserId = complaint.SenderUserId,
            TargetUserId = complaint.TargetUserId,
            TargetUserName = complaint.TargetUser?.FullName ?? complaint.TargetUser?.UserName ?? "Unknown",
            MessageId = complaint.MessageId,
            PostId = complaint.PostId,
            Reason = (int)complaint.Reason,
            ReasonText = complaint.ReasonText,
            CreatedAt = complaint.CreatedAt,
            Status = (int)complaint.Status,
            ReviewedByAdminId = complaint.ReviewedByAdminId,
            ReviewedAt = complaint.ReviewedAt,
            AdminComment = complaint.AdminComment,
            IsBanned = complaint.IsBanned,
            IsWarned = complaint.IsWarned,
            UpdatedAt = complaint.UpdatedAt,
            MessageContent = complaint.Message?.Text,
            PostContent = complaint.Post?.Content
        };
    }

    private async Task SendAdminNotificationAsync(Complaint complaint, string notificationType)
    {
        // Получаем всех администраторов
        var admins = await _context.Users
            .Where(u => u.UserType == "admin")
            .ToListAsync();

        foreach (var admin in admins)
        {
            var notification = new Notification
            {
                UserId = admin.Id,
                Title = "Новая жалоба",
                Message = $"Новая жалоба на пользователя {complaint.TargetUser?.FullName ?? "Unknown"}",
                Type = NotificationType.ComplaintCreated,
                IsRead = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ActionUrl = $"/admin/complaints/{complaint.Id}",
                ActionText = "Просмотреть"
            };

            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();
    }

    private async Task SendStatusChangeNotificationAsync(Complaint complaint, ComplaintStatus previousStatus)
    {
        // Получаем всех администраторов для уведомления
        var admins = await _context.Users
            .Where(u => u.UserType == "admin")
            .ToListAsync();

        var statusText = complaint.Status switch
        {
            ComplaintStatus.Reviewed => "просмотрена",
            ComplaintStatus.Resolved => "разрешена",
            ComplaintStatus.Dismissed => "отклонена",
            _ => "обновлена"
        };

        foreach (var admin in admins)
        {
            if (admin.Id != complaint.ReviewedByAdminId) // Не отправляем тому кто сделал действие
            {
                var notification = new Notification
                {
                    UserId = admin.Id,
                    Title = "Жалоба обновлена",
                    Message = $"Жалоба #{complaint.Id} {statusText}",
                    Type = NotificationType.ComplaintUpdated,
                    IsRead = false,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ActionUrl = $"/admin/complaints/{complaint.Id}",
                    ActionText = "Просмотреть"
                };

                _context.Notifications.Add(notification);
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task SendUserWarningNotificationAsync(string userId)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = "Предупреждение",
            Message = "Ваш контент нарушил правила сообщества и был отмечен администратором",
            Type = NotificationType.Warning,
            IsRead = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}
