using System.Collections.Generic;
using DeepseekAPI.Messages;
using DeepseekAPI.Tools;
using Newtonsoft.Json;

namespace DeepseekAPI.Schema
{
    internal sealed class ChatCompletionRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; } = null!;

        [JsonProperty("messages")]
        public IEnumerable<Message> Messages { get; set; } = null!;

        [JsonProperty("temperature")]
        public double? Temperature { get; set; }

        [JsonProperty("top_p")]
        public double? TopP { get; set; }

        [JsonProperty("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }

        [JsonProperty("presence_penalty")]
        public double? PresencePenalty { get; set; }

        [JsonProperty("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonProperty("stop")]
        public StopOption? Stop { get; set; }

        [JsonProperty("stream")]
        public bool? Stream { get; set; }

        [JsonProperty("thinking")]
        public ThinkingConfig? Thinking { get; set; }

        [JsonProperty("reasoning_effort")]
        public string? ReasoningEffort { get; set; }

        [JsonProperty("response_format")]
        public ResponseFormatConfig? ResponseFormat { get; set; }

        [JsonProperty("logprobs")]
        public bool? Logprobs { get; set; }

        [JsonProperty("top_logprobs")]
        public int? TopLogprobs { get; set; }

        [JsonProperty("tools")]
        public IEnumerable<ToolDefinition>? Tools { get; set; }

        [JsonProperty("tool_choice")]
        public ToolChoice? ToolChoiceOption { get; set; }

        [JsonProperty("user_id")]
        public string? UserId { get; set; }
        
        internal sealed class ThinkingConfig
        {
            [JsonProperty("type")]
            public string Type { get; set; } = null!;

            public ThinkingConfig(ThinkingMode mode)
            {
                Type = mode.AsParameterString();
            }
        }

        internal sealed class ResponseFormatConfig
        {
            [JsonProperty("type")]
            public string Type { get; set; } = null!;

            public ResponseFormatConfig(ResponseFormatType format)
            {
                Type = format.AsParameterString();
            }
        }
    }
}
