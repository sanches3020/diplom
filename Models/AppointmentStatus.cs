namespace Sofia.Web.Models
{
    public enum AppointmentStatus
    {
        Scheduled = 0,   // Запланировано
        Confirmed = 1,   // Подтверждено
        Completed = 2,   // Завершено
        Cancelled = 3,   // Отменено
        NoShow = 4   // Не явился
    }
}

