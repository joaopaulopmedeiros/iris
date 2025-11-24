using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Converters;

public class DecimalStringJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return decimal.Parse(value!, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}