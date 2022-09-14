namespace QuietRoom.Server.Models;

/// <summary>
/// Represents an event
/// </summary>
public record Event(string Name, string BuildingCode, string RoomNumber, string StartTime, string EndTime, DayOfWeek[] DaysMet,
                    DateOnly StartDate, DateOnly EndDate);
