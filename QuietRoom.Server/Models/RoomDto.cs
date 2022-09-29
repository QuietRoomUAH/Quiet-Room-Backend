using System.Collections.Immutable;
using System.Text.Json;
using QuietRoom.Server.Utils.JsonConverters;

namespace QuietRoom.Server.Models;

/// <summary>
/// Represents a room as a data transfer object
/// </summary>
public record RoomDto(string BuildingCode, string RoomNumber, int Capacity, string? RoomType, ImmutableList<EventDto> Events)
{
    public string ToJson()
    {
        return JsonSerializer.SerializeToElement(this,
            new JsonSerializerOptions
            {
                Converters = { new TimeOnlyJsonConverter(), new DateOnlyJsonConverter(), new DayOfWeekJsonConverter() }
            }).ToString();
    }
}
