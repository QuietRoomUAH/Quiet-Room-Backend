using QuietRoom.Server.Models;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Startup;

/// <summary>
/// Extensions for using test data in the application.
/// </summary>
public static class TestStartupExtensions
{
    public static IServiceCollection UseTestRoomRetriever(this IServiceCollection services)
    {
        services.AddSingleton<IRoomRepository, TestRoomRepository>();
        return services;
    }
    
    public class TestRoomRepository : IRoomRepository
    {
        /// <inheritdoc />
        public Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, TimeOnly endTime, DayOfWeek day)
        {
            var roomCodes = new[] { "101", "102", "103" };
            var roomDtos = roomCodes.Select(roomCode => new RoomDto(buildingCode, roomCode, 0, null));
            return Task.FromResult(roomDtos);
        }
    }
}
