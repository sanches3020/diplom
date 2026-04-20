namespace Sofia.Web.DTO.Complaints;

public class UpdateComplaintStatusRequest
{
    /// <summary>
    /// Новый статус жалобы (0=New, 1=Reviewed, 2=Resolved, 3=Dismissed)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Комментарий администратора
    /// </summary>
    public string? AdminComment { get; set; }

    /// <summary>
    /// Применить ли бан
    /// </summary>
    public bool ApplyBan { get; set; } = false;

    /// <summary>
    /// Применить ли предупреждение
    /// </summary>
    public bool ApplyWarning { get; set; } = false;
}
