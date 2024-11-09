namespace dashboard.Data;

public class Log
{
    public Guid Id { get; set; }
    public LogType LogType { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
    public Person Person { get; set; }
}

public enum LogType
{
    CheckIn,
    CheckOut,
    SickLeave,
    UnpaidLeave,
    Vacation,
    PaidLeave
}

public static class LogTypeExtensions
{
    public static string GetDisplayName(this LogType logType) => logType switch
    {
        LogType.CheckIn => "Check In",
        LogType.CheckOut => "Check Out",
        LogType.SickLeave => "Sick Leave",
        LogType.UnpaidLeave => "Unpaid Leave",
        LogType.Vacation => "Vacation",
        LogType.PaidLeave => "Paid Leave",
        _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
    };
}
