using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace QuietRoom.Server.Models.Entities;

#pragma warning disable CS8618
[Table("rooms"), UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class RoomEntity
{
    public string BuildingCode { get; set; }
    public string RoomNumber { get; set; }
    public int Capacity { get; set; }
    public string RoomType { get; set; }
    
    public List<EventEntity> Events { get; set; }

    /// <summary>
    /// Converts a room to a DTO
    /// </summary>
    /// <param name="daysMetLookup">A lookup that mimics a multimap, to allow retrieving the days met for a given event id</param>
    /// <returns></returns>
    public RoomDto ToDto(ILookup<Guid, DayOfWeek> daysMetLookup)
    {
        var eventDtos = Events
            .Select(eventEntity => eventEntity.ToDto(daysMetLookup[eventEntity.Id].ToImmutableHashSet()))
            .ToImmutableList();
        return new RoomDto(BuildingCode, RoomNumber, Capacity, RoomType, eventDtos);
    }
}