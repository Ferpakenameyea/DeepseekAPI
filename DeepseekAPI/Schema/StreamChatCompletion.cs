using System.Collections.Generic;
using DeepseekAPI.Messages;
using Newtonsoft.Json;

namespace DeepseekAPI.Schema
{
    public sealed class StreamChatCompletion
    {
        [JsonProperty("id")]
        public string Id { get; private set; } = null!;
        [JsonProperty("created")]
        public int Created { get; private set; }
        [JsonProperty("model")]
        public string Model { get; private set; } = null!;
        [JsonProperty("system_fingerprint")]
        public string SystemFingerprint { get; private set; } = null!;
        [JsonProperty("object")]
        public string Object { get; private set; } = null!;
        [JsonProperty("choices")]
        public IReadOnlyList<StreamChoice> Choices { get; private set; } = null!;
        [JsonProperty("usage")]
        public ChatCompletion.UsageData? Usage { get; private set; }

        public sealed class StreamChoice
        {
            [JsonProperty("index")]
            public int Index { get; private set; }
            [JsonProperty("delta")]
            public Delta Delta { get; private set; } = null!;
            [JsonProperty("finish_reason")]
            public string? FinishReason { get; private set; }
            [JsonProperty("logprobs")]
            public ChatCompletion.LogprobsData? Logprobs { get; private set; }
        }

        public sealed class Delta
        {
            [JsonProperty("role")]
            public string? Role { get; private set; }
            [JsonProperty("content")]
            public string? Content { get; private set; }
            [JsonProperty("reasoning_content")]
            public string? ReasoningContent { get; private set; }
            [JsonProperty("tool_calls")]
            public IReadOnlyList<ToolCall>? ToolCalls { get; private set; }
        }
    }
}
