using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using QuietRoom.Server.Models;
using QuietRoom.Server.Services.Interfaces;

namespace QuietRoom.Server.Services.Db;

public class SupabaseDbManager : IBuildingRepository, IRoomRepository
{
    private readonly SupabaseDbContext _dbContext;
    private readonly ITimeProvider _timeProvider;

    public SupabaseDbManager(SupabaseDbContext dbContext, ITimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
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
        var emptyList = ImmutableList<EventDto>.Empty;
        var roomDtos = rooms.Select(roomEntity => new RoomDto(roomEntity.BuildingCode, roomEntity.RoomNumber,
            roomEntity.Capacity, roomEntity.RoomType, emptyList));
        return await roomDtos.ToArrayAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsAsync(string buildingCode, TimeOnly startTime, 
                                                                   TimeOnly endTime, DayOfWeek dayOfWeek)
    {
        var now = _timeProvider.Now;
        var emptyList = ImmutableList<EventDto>.Empty;
        var dateNow = DateOnly.FromDateTime(now.DateTime);
        var eventsOccurring = await _dbContext.Events
            .Where(eventEntity => eventEntity.BuildingCode == buildingCode)
            .Where(eventEntity => eventEntity.StartDate <= dateNow && eventEntity.EndDate >= dateNow)
            .Where(eventEntity => eventEntity.StartTime <= endTime && eventEntity.EndTime >= startTime)
            .ToListAsync();
        var eventIds = eventsOccurring.Select(eventEntity => eventEntity.Id).ToArray();

        var invalidEventIds = await _dbContext.DaysMet
            .Where(dayMetEntity => eventIds.Contains(dayMetEntity.Event.Id))
            .Where(dayMetEntity => !dayMetEntity.DaysMet.Contains(dayOfWeek.ToString().ToUpper()))
            .Select(dayMetEntity => dayMetEntity.Event.Id)
            .ToListAsync();
        
        var validEventIds = eventIds.Except(invalidEventIds).ToImmutableHashSet();
        
        var roomNumbers = eventsOccurring
            .Where(eventEntity => validEventIds.Contains(eventEntity.Id))
            .Select(eventEntity => eventEntity.RoomNumber);
        
        var openRooms = await _dbContext.Rooms
            .Where(roomEntity => roomEntity.BuildingCode == buildingCode)
            .Where(roomEntity => !roomNumbers.Contains(roomEntity.RoomNumber))
            .Select(roomEntity => new RoomDto(roomEntity.BuildingCode, roomEntity.RoomNumber, roomEntity.Capacity, 
                roomEntity.RoomType, emptyList))
            .ToListAsync();
        return openRooms;
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

    /// <inheritdoc />
    public async Task<bool> IsRoomAvailableAsync(string buildingCode, string roomNumber)
    {
        var room = await GetRoomInfoAsync(buildingCode, roomNumber);
        if (room is null) return false;
        var now = _timeProvider.Now;
        var nowTime = TimeOnly.FromDateTime(now.DateTime);
        var nowDate = DateOnly.FromDateTime(now.DateTime);
        var today = now.DayOfWeek;
        var areEventsNow = room.Events.Any(eventDto => 
            eventDto.StartDate <= nowDate && 
            eventDto.EndDate >= nowDate && 
            eventDto.StartTime <= nowTime && 
            eventDto.EndTime >= nowTime &&
            eventDto.DaysMet.Contains(today));
        return !areEventsNow;
    }
}
