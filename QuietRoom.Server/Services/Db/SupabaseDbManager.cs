using System.Collections.Immutable;
using QuietRoom.Server.Models;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Services.Db;

public class SupabaseDbManager : IBuildingRepository, IRoomRepository
{
    private readonly SupabaseDbContext _dbContext;
    
    public SupabaseDbManager(SupabaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> GetBuildingNamesAsync()
    {
        var buildings = _dbContext.Rooms
            .Select(entity => entity.BuildingCode)
            .Distinct()
            .ToImmutableHashSet();
        return Task.FromResult<IEnumerable<string>>(buildings);
    }

    /// <inheritdoc />
    public Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, TimeOnly endTime,
                                                                  DayOfWeek dayOfWeek)
    {
        var rooms = _dbContext.DaysMet
            .Where(dayMet => dayMet.DaysMet == dayOfWeek.ToString().ToUpper())
            .Select(dayMet => dayMet.Event)
            .Where(room => room.Room.BuildingCode == buildingCode)
            .Where(room => room.StartTime < endTime)
            .Where(room => room.EndTime > startTime)
            .Where(room => room.StartDate < DateOnly.FromDateTime(DateTime.Now))
            .Where(room => room.EndDate > DateOnly.FromDateTime(DateTime.Now))
            .Select(entity => entity.Room)
            .ToImmutableHashSet();
        var roomDtos = rooms.Select(roomEntity => 
            new RoomDto(roomEntity.BuildingCode, roomEntity.RoomNumber, roomEntity.Capacity, roomEntity.RoomType));
        return Task.FromResult(roomDtos);
    }
}
