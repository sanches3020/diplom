namespace Sofia.Web.Models;

public enum NotificationType
{
    System = 0,
    Reminder = 1,
    Alert = 2,
    Message = 3
}

public enum NotificationPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}

public enum AppointmentStatus
{
    Scheduled = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3,
    Missed = 4
}

public enum AnswerType
{
    SingleChoice = 0,
    MultipleChoice = 1,
    FreeText = 2
}

public enum TestType
{
    BuiltIn = 0,
    Custom = 1
}
