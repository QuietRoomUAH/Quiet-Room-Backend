using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Startup;

/// <summary>
/// Extensions for using test data in the application.
/// </summary>
public static class TestStartupExtensions
{
    public static IServiceCollection UseTestRoomRetriever(this IServiceCollection services)
    {
        services.AddSingleton<IRoomRetriever, TestRoomRetriever>();
        return services;
    }
    
    public class TestRoomRetriever : IRoomRetriever
    {
        /// <inheritdoc />
        public Task<IEnumerable<string>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, TimeOnly endTime)
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "101", "102", "103" });
        }
    }
}
