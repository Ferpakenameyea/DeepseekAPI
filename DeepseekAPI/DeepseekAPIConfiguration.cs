using System;
using DeepseekAPI.Schema;

namespace DeepseekAPI
{
    public sealed class DeepseekAPIConfiguration
    {
        public string Model { get; }
        public double? Temperature { get; }
        public double? TopP { get; }
        public double? FrequencyPenalty { get; }
        public double? PresencePenalty { get; }
        public int? MaxTokens { get; }
        public StopOption? Stop { get; }
        public bool? Thinking { get; }
        public ReasoningEffortType? ReasoningEffort { get; }
        public ResponseFormatType? ResponseFormat { get; }
        public bool? Logprobs { get; }
        public int? TopLogprobs { get; }
        public string? UserId { get; }

        public DeepseekAPIConfiguration(
            string model,
            double? temperature                  = null,
            double? topP                         = null,
            double? frequencyPenalty             = null,
            double? presencePenalty              = null,
            int? maxTokens                       = null,
            StopOption? stop                     = null,
            bool? thinking                       = null,
            ReasoningEffortType? reasoningEffort = null,
            ResponseFormatType? responseFormat   = null,
            bool? logprobs                       = null,
            int? topLogprobs                     = null,
            string? userId                       = null)
        {
            Model            = model;
            Temperature      = temperature;
            TopP             = topP;
            FrequencyPenalty = frequencyPenalty;
            PresencePenalty  = presencePenalty;
            MaxTokens        = maxTokens;
            Stop             = stop;
            Thinking         = thinking;
            ReasoningEffort  = reasoningEffort;
            ResponseFormat   = responseFormat;
            Logprobs         = logprobs;
            TopLogprobs      = topLogprobs;
            UserId           = userId;
        }
    }

    public enum ReasoningEffortType
    {
        High, Max
    }

    internal static class ReasoningEffortTypeExtension
    {
        public static string AsParameterString(this ReasoningEffortType self)
        {
            return self switch
            {
                ReasoningEffortType.High => "high",
                ReasoningEffortType.Max => "max",
                _ => throw new ArgumentException($"Unknown {nameof(ReasoningEffortType)}: {self}")
            };
        }
    }

    public enum ThinkingMode
    {
        Enabled, Disabled
    }

    internal static class ThinkingModeExtension
    {
        public static string AsParameterString(this ThinkingMode self)
        {
            return self switch
            {
                ThinkingMode.Enabled => "enabled",
                ThinkingMode.Disabled => "disabled",
                _ => throw new ArgumentException($"Unknown {nameof(ThinkingMode)}: {self}")
            };
        }
    }

    public enum ResponseFormatType
    {
        JsonObject, Text
    }

    internal static class ResponseFormatTypeExtension
    {
        public static string AsParameterString(this ResponseFormatType self)
        {
            return self switch
            {
                ResponseFormatType.JsonObject => "json_object",
                ResponseFormatType.Text => "text",
                _ => throw new ArgumentException($"Unknown {nameof(ResponseFormatType)}: {self}")
            };
        }
    }
}
