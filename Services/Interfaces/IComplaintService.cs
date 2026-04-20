using Sofia.Web.DTO.Complaints;
using Sofia.Web.Models;

namespace Sofia.Web.Services.Interfaces;

public interface IComplaintService
{
    /// <summary>
    /// Создать жалобу
    /// </summary>
    Task<ComplaintDto?> CreateComplaintAsync(string senderUserId, CreateComplaintRequest request);

    /// <summary>
    /// Получить все жалобы (только для администраторов)
    /// </summary>
    Task<List<ComplaintDto>> GetAllComplaintsAsync(int? statusFilter = null);

    /// <summary>
    /// Получить жалобы на конкретного пользователя
    /// </summary>
    Task<List<ComplaintDto>> GetComplaintsOnUserAsync(string targetUserId);

    /// <summary>
    /// Получить жалобу по ID
    /// </summary>
    Task<ComplaintDto?> GetComplaintByIdAsync(int complaintId);

    /// <summary>
    /// Обновить статус жалобы
    /// </summary>
    Task<bool> UpdateComplaintStatusAsync(int complaintId, string adminId, UpdateComplaintStatusRequest request);

    /// <summary>
    /// Получить количество незавершенных жалоб
    /// </summary>
    Task<int> GetUnreviewedCountAsync();

    /// <summary>
    /// Получить статистику по жалобам
    /// </summary>
    Task<ComplaintStatsDto> GetComplaintStatsAsync();

    /// <summary>
    /// Проверить, может ли пользователь подать жалобу на этот контент
    /// </summary>
    Task<(bool canComplain, string? reason)> CanComplainAsync(string senderUserId, string targetUserId, string? messageId = null, int? postId = null);
}

public class ComplaintStatsDto
{
    public int TotalComplaints { get; set; }
    public int NewComplaints { get; set; }
    public int ReviewedComplaints { get; set; }
    public int ResolvedComplaints { get; set; }
    public int DismissedComplaints { get; set; }
    public int UsersWithComplaints { get; set; }
    public int UsersWithMultipleComplaints { get; set; }
}
