using System.Text.Json;
using System.Text.Json.Serialization;

namespace AupInfo.Core
{
    public class PsdToolKitImageConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                return null;

            string? filePath = null;
            var startDepth = reader.CurrentDepth;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject && reader.CurrentDepth == startDepth)
                    return filePath;
                if (reader.TokenType != JsonTokenType.PropertyName || filePath != null)
                    continue;

                if (reader.GetString()!.ToLower() != "filepath")
                    continue;
                if (!reader.Read())
                    throw new JsonException();

                filePath = reader.TokenType switch
                {
                    JsonTokenType.String => reader.GetString(),
                    _ => null,
                };
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
