namespace Sofia.Web.DTO.Complaints;

public class CreateComplaintRequest
{
    /// <summary>
    /// ID пользователя, на которого жалоба
    /// </summary>
    public string TargetUserId { get; set; } = "";

    /// <summary>
    /// ID сообщения чата (опционально)
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// ID поста форума (опционально)
    /// </summary>
    public int? PostId { get; set; }

    /// <summary>
    /// Причина жалобы
    /// </summary>
    public int Reason { get; set; } // 0=Harassment, 1=Spam, 2=AdultContent, etc.

    /// <summary>
    /// Дополнительный текст причины
    /// </summary>
    public string? ReasonText { get; set; }
}
