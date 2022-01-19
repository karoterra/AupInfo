using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AupInfo.Core
{
    public class PsdToolKitThumbnailConverter : JsonConverter<Bitmap?>
    {
        public override Bitmap? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                return null;

            string s = reader.GetString()!;
            var data = Convert.FromBase64String(s);
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);
            bw.Write(data);
            ms.Position = 0;
            Bitmap bitmap = new(ms);
            return bitmap;
        }

        public override void Write(Utf8JsonWriter writer, Bitmap? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
