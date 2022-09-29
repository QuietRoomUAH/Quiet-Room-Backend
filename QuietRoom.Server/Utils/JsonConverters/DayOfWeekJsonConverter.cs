using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuietRoom.Server.Utils.JsonConverters;

/// <summary>
/// Converter for days of the week that uses the string value of the enum
/// </summary>
public class DayOfWeekJsonConverter : JsonConverter<DayOfWeek>
{
    /// <inheritdoc />
    public override DayOfWeek Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DayOfWeek value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
