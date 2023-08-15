namespace QuietRoom.Server.Models;

/// <summary>
/// Represents an event
/// </summary>
public record EventDto(string Name, string BuildingCode, string RoomNumber, TimeOnly StartTime, TimeOnly EndTime, 
                       DayOfWeek[] DaysMet,DateOnly StartDate, DateOnly EndDate);
