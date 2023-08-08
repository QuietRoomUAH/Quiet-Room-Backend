using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
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
    public async Task<IEnumerable<RoomDto>> GetRoomNumbersAsync(string buildingCode)
    {
        var rooms = _dbContext.Rooms
            .Where(roomEntity => roomEntity.BuildingCode == buildingCode);
        var roomDtos = rooms.Select(roomEntity => new RoomDto(roomEntity.BuildingCode, roomEntity.RoomNumber,
            roomEntity.Capacity, roomEntity.RoomType, ImmutableList<EventDto>.Empty));
        return await roomDtos.ToArrayAsync();
    }

    /// <inheritdoc />
    public Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, TimeOnly endTime,
                                                                  DayOfWeek dayOfWeek)
    {
        var rooms = _dbContext.DaysMet
            .Where(dayMet => dayMet.DaysMet == dayOfWeek.ToString().ToUpper())
            .Select(dayMet => dayMet.Event)
            .Where(room => room.Room.BuildingCode == buildingCode)
            .Where(room => !(room.StartTime < endTime && room.EndTime > startTime))
            .Where(room => room.StartDate < DateOnly.FromDateTime(DateTime.Now))
            .Where(room => room.EndDate > DateOnly.FromDateTime(DateTime.Now))
            .Select(entity => entity.Room)
            .ToImmutableHashSet();
        var roomDtos = rooms.Select(roomEntity => 
            new RoomDto(roomEntity.BuildingCode, roomEntity.RoomNumber, roomEntity.Capacity, roomEntity.RoomType, ImmutableList<EventDto>.Empty));
        return Task.FromResult(roomDtos);
    }

    /// <inheritdoc />
    public Task<RoomDto?> GetRoomInfoAsync(string buildingCode, string roomNumber)
    {
        // Get the first room that matches the building code and room number. Additionally, include its events.
        var roomEntity = _dbContext.Rooms
            .Where(room => room.BuildingCode == buildingCode)
            .Where(room => room.RoomNumber == roomNumber)
            .Include(room => room.Events)
            .FirstOrDefault();
        // If the room couldn't be found, return null.
        if (roomEntity is null)
        {
            return Task.FromResult((RoomDto?) null);
        }
        // Get all of the event ids for the room
        var eventIds = roomEntity.Events
            .Select(eventEntity => eventEntity.Id)
            .ToImmutableList();
        // Get all of the days met rows that match one of the event ids for the room
        var daysMet = _dbContext.DaysMet
            .Where(dayMet => eventIds.Contains(dayMet.Event.Id))
            .Include(dayMet => dayMet.Event)
            .ToLookup(dayMet => dayMet.Event.Id, dayMet => Enum.Parse<DayOfWeek>(dayMet.DaysMet, true));
        // Sort the events in the room by their start date, then by their start time
        roomEntity.Events = roomEntity.Events
            .OrderBy(eventEntity => eventEntity.StartDate)
            .ThenBy(eventEntity => eventEntity.StartTime)
            .ToList();
        // Create a room dto from the entity
        var roomDto = roomEntity.ToDto(daysMet);
        return Task.FromResult((RoomDto?) roomDto);
    }
}
