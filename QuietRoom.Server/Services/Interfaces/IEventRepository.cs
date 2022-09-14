using QuietRoom.Server.Models;

namespace QuietRoom.Server.Services.Interfaces;

/// <summary>
/// Repository for grabbing events from some database
/// </summary>
public interface IEventRepository
{
    /// <summary>
    /// Gets all events that occur in a specific room
    /// </summary>
    /// <param name="buildingCode">The building to get the events from</param>
    /// <param name="roomNumber">The room to get the events from</param>
    /// <param name="date">The date of the events</param>
    /// <returns>A list of events, may or may not be sorted</returns>
    public IEnumerable<Event> GetEvent(string buildingCode, string roomNumber, DateOnly? date);
}
