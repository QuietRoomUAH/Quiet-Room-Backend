using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JetBrains.Annotations;

namespace QuietRoom.Server.Models.Entities;

// Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8618 
[Table("events"), UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class EventEntity
{
    [Key]
    public Guid Id { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Name { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Term { get; set; }
    
    public string RoomNumber { get; set; }
    public string BuildingCode { get; set; }
    public RoomEntity Room { get; set; }

    [Table("event_days"), UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DayMetEntity
    {
        public EventEntity Event { get; set; }
        public string DaysMet { get; set; }
    }

    public EventDto ToDto(IEnumerable<DayOfWeek> daysMet)
    {
        return new EventDto
        (
            Name,
            BuildingCode,
            RoomNumber,
            StartTime,
            EndTime,
            daysMet.ToArray(),
            StartDate,
            EndDate
        );
    }
}
