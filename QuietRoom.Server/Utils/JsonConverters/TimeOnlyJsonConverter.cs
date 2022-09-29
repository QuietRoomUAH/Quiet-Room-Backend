using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuietRoom.Server.Utils.JsonConverters;

/// <summary>
/// Class to allow converting a time only to a string
/// </summary>
public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    /// <inheritdoc />
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToBasicString());
    }
}
