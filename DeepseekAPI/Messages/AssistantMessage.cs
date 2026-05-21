using System.Collections.Generic;
using Newtonsoft.Json;

namespace DeepseekAPI.Messages
{
    public sealed class AssistantMessage : Message
    {
        public const string AssistantRoleName = "assistant";
        public override string Role => AssistantRoleName;
        [JsonProperty("name")]
        public string? Name { get; }
        [JsonProperty("prefix")]
        public bool? Prefix { get; }
        [JsonProperty("reasoning_content")]
        public string? ReasoningContent { get; }
        [JsonProperty("tool_calls")]
        public IReadOnlyList<ToolCall>? ToolCalls { get; }

        public AssistantMessage(
            string? content = null,
            string? name = null,
            bool? prefix = null,
            string? reasoningContent = null,
            IReadOnlyList<ToolCall>? toolCalls = null
        ) : base(content ?? string.Empty)
        {
            Name = name;
            Prefix = prefix;
            ReasoningContent = reasoningContent;
            ToolCalls = toolCalls;
        }
    }

    public sealed class ToolCall
    {
        [JsonProperty("id")]
        public string Id { get; private set; }
        [JsonProperty("type")]
        public string Type { get; private set; } = "function";
        [JsonProperty("function")]
        public FunctionCall Function { get; private set; }

        [JsonConstructor]
        public ToolCall(string id, string type, FunctionCall function)
        {
            Id = id;
            Type = type;
            Function = function;
        }

        public ToolCall(string id, string functionName, string arguments)
            : this(id, "function", new FunctionCall(functionName, arguments))
        {
        }
    }

    public sealed class FunctionCall
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("arguments")]
        public string Arguments { get; private set; }

        [JsonConstructor]
        public FunctionCall(string name, string arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
