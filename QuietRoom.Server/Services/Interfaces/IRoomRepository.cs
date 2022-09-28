using QuietRoom.Server.Models;

namespace QuietRoom.Server.Services.Interfaces;

/// <summary>
/// Interface that retrieves rooms from the database
/// </summary>
public interface IRoomRepository
{
    /// <summary>
    /// Gets available rooms based on the given criteria
    /// </summary>
    /// <param name="buildingCode">The building to search for available rooms in</param>
    /// <param name="startTime">The start of the time window for which there should be no events</param>
    /// <param name="endTime">The end of the time window for which there should be no events</param>
    /// <param name="dayOfWeek">The day to look for</param>
    /// <returns>A list of room numbers. This can be empty if the building doesn't exist</returns>
    public Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, TimeOnly endTime, 
                                                             DayOfWeek dayOfWeek);
}
