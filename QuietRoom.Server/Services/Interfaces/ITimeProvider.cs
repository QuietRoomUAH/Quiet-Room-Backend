namespace QuietRoom.Server.Services.Interfaces;

/// <summary>Interface that represents a way to get the current time and date for a time zone</summary>
public interface ITimeProvider
{
    /// <summary>Get the current time with respect to the time zone</summary>
    public DateTimeOffset Now { get; }
    
    /// <summary>The relevant time zone</summary>
    public TimeZoneInfo TimeZone { get; }
}
