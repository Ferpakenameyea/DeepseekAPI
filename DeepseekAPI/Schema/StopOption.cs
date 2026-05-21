using System;
using Newtonsoft.Json;

namespace DeepseekAPI.Schema
{
    [JsonConverter(typeof(StopOptionConverter))]
    public struct StopOption
    {
        internal string[] Values { get; }

        public StopOption(string stop)
        {
            Values = new[] { stop };
        }

        public StopOption(string[] stops)
        {
            Values = stops;
        }

        public static implicit operator StopOption(string stop) => new StopOption(stop);
        public static implicit operator StopOption(string[] stops) => new StopOption(stops);
    }

    internal sealed class StopOptionConverter : JsonConverter<StopOption>
    {
        public override StopOption ReadJson(
            JsonReader reader,
            Type objectType,
            StopOption existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return reader.TokenType switch
            {
                JsonToken.String => new StopOption((string)reader.Value!),
                JsonToken.StartArray => new StopOption(serializer.Deserialize<string[]>(reader) ?? Array.Empty<string>()),
                _ => throw new JsonSerializationException($"Unexpected token type for StopOption: {reader.TokenType}")
            };
        }

        public override void WriteJson(JsonWriter writer, StopOption value, JsonSerializer serializer)
        {
            if (value.Values.Length == 1)
            {
                writer.WriteValue(value.Values[0]);
            }
            else
            {
                serializer.Serialize(writer, value.Values);
            }
        }
    }
}
