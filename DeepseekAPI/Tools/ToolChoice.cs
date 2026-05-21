using System;
using Newtonsoft.Json;

namespace DeepseekAPI.Tools
{
    [JsonConverter(typeof(ToolChoiceConverter))]
    public abstract class ToolChoice
    {
        public static ToolChoice None { get; } = new ChatCompletionToolChoice("none");
        public static ToolChoice Auto { get; } = new ChatCompletionToolChoice("auto");
        public static ToolChoice Required { get; } = new ChatCompletionToolChoice("required");
    }

    public sealed class ChatCompletionToolChoice : ToolChoice
    {
        public string Value { get; }

        public ChatCompletionToolChoice(string value)
        {
            Value = value;
        }
    }

    public sealed class ChatCompletionNamedToolChoice : ToolChoice
    {
        public string FunctionName { get; }

        public ChatCompletionNamedToolChoice(string functionName)
        {
            FunctionName = functionName;
        }
    }

    internal sealed class ToolChoiceConverter : JsonConverter<ToolChoice>
    {
        public override ToolChoice ReadJson(
            JsonReader reader,
            Type objectType,
            ToolChoice? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new ChatCompletionToolChoice((string)reader.Value!);
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                var obj = serializer.Deserialize<JRawNamedToolChoice>(reader);
                return new ChatCompletionNamedToolChoice(obj!.Function!.Name!);
            }

            throw new JsonSerializationException($"Unexpected token for ToolChoice: {reader.TokenType}");
        }

        public override void WriteJson(JsonWriter writer, ToolChoice? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            switch (value)
            {
                case ChatCompletionToolChoice stringChoice:
                    writer.WriteValue(stringChoice.Value);
                    break;
                case ChatCompletionNamedToolChoice namedChoice:
                    serializer.Serialize(writer, new
                    {
                        type = "function",
                        function = new { name = namedChoice.FunctionName }
                    });
                    break;
                default:
                    throw new JsonSerializationException($"Unknown ToolChoice type: {value.GetType()}");
            }
        }

        private sealed class JRawNamedToolChoice
        {
            [JsonProperty("function")]
            public JRawFunctionName? Function { get; set; }
        }

        private sealed class JRawFunctionName
        {
            [JsonProperty("name")]
            public string? Name { get; set; }
        }
    }
}
