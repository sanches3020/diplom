using Sofia.Web.Models;

namespace Sofia.Web.DTO.Complaints;

public class ComplaintDto
{
    public int Id { get; set; }
    public string SenderUserId { get; set; } = "";
    public string TargetUserId { get; set; } = "";
    public string TargetUserName { get; set; } = "";
    public string? MessageId { get; set; }
    public int? PostId { get; set; }
    public int Reason { get; set; }
    public string? ReasonText { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Status { get; set; } // 0=New, 1=Reviewed, 2=Resolved, 3=Dismissed
    public string? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? AdminComment { get; set; }
    public bool IsBanned { get; set; }
    public bool IsWarned { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Контент жалобы для администратора
    public string? MessageContent { get; set; }
    public string? PostContent { get; set; }
}
