using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuietRoom.Server.Utils.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    /// <inheritdoc />
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        var date = value.ToString("yyyy-MM-dd");
        writer.WriteStringValue(date);
    }
}
