using Newtonsoft.Json;

namespace DeepseekAPI.Messages
{
    public sealed class ToolMessage : Message
    {
        public const string ToolRoleName = "tool";
        public override string Role => ToolRoleName;
        [JsonProperty("tool_call_id")]
        public string ToolCallId { get; }

        public ToolMessage(string toolCallId, string content)
            : base(content)
        {
            ToolCallId = toolCallId;
        }
    }
}
