using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Services;

public class SimpleTimeProvider : ITimeProvider
{
    /// <inheritdoc />
    public DateTimeOffset Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone);
    
    /// <inheritdoc />
    public TimeZoneInfo TimeZone => TimeZoneInfo.FindSystemTimeZoneById("US/Central");
}
