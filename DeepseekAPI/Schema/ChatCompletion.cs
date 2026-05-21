using System.Collections.Generic;
using DeepseekAPI.Messages;
using Newtonsoft.Json;

namespace DeepseekAPI.Schema
{
    public sealed class ChatCompletion
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
        public IReadOnlyList<Choice> Choices { get; private set; } = null!;
        [JsonProperty("usage")]
        public UsageData Usage { get; private set; } = null!;

        public sealed class Choice
        {
            [JsonProperty("index")]
            public int Index { get; private set; }
            [JsonProperty("message")]
            public ChatCompletionMessage Message { get; private set; } = null!;
            [JsonProperty("finish_reason")]
            public string FinishReason { get; private set; } = null!;
            [JsonProperty("logprobs")]
            public LogprobsData? Logprobs { get; private set; }
        }

        public sealed class ChatCompletionMessage
        {
            [JsonProperty("role")]
            public string Role { get; private set; } = null!;
            [JsonProperty("content")]
            public string? Content { get; private set; }
            [JsonProperty("reasoning_content")]
            public string? ReasoningContent { get; private set; }
            [JsonProperty("tool_calls")]
            public IReadOnlyList<ToolCall>? ToolCalls { get; private set; }
        }

        public sealed class LogprobsData
        {
            [JsonProperty("content")]
            public IReadOnlyList<LogprobContentEntry>? Content { get; private set; }
        }

        public sealed class LogprobContentEntry
        {
            [JsonProperty("token")]
            public string Token { get; private set; } = null!;
            [JsonProperty("logprob")]
            public double Logprob { get; private set; }
            [JsonProperty("bytes")]
            public IReadOnlyList<int>? Bytes { get; private set; }
            [JsonProperty("top_logprobs")]
            public IReadOnlyList<TopLogprobEntry> TopLogprobs { get; private set; } = null!;
        }

        public sealed class TopLogprobEntry
        {
            [JsonProperty("token")]
            public string Token { get; private set; } = null!;
            [JsonProperty("logprob")]
            public double Logprob { get; private set; }
            [JsonProperty("bytes")]
            public IReadOnlyList<int>? Bytes { get; private set; }
        }

        public sealed class UsageData
        {
            [JsonProperty("completion_tokens")]
            public int CompletionTokens { get; private set; }
            [JsonProperty("prompt_tokens")]
            public int PromptTokens { get; private set; }
            [JsonProperty("prompt_cache_hit_tokens")]
            public int PromptCacheHitTokens { get; private set; }
            [JsonProperty("prompt_cache_miss_tokens")]
            public int PromptCacheMissTokens { get; private set; }
            [JsonProperty("total_tokens")]
            public int TotalTokens { get; private set; }
            [JsonProperty("completion_tokens_details")]
            public CompletionTokensDetails? CompletionTokensDetails { get; private set; }
        }

        public sealed class CompletionTokensDetails
        {
            [JsonProperty("reasoning_tokens")]
            public int ReasoningTokens { get; private set; }
        }
    }
}
