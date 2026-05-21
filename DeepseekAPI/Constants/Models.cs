using System;

namespace DeepseekAPI.Constants
{
    public static class Models
    {
        public const string V4FlashModel = "deepseek-v4-flash";
        public const string V4ProModel = "deepseek-v4-pro";

        [Obsolete("Deprecating in 2026.07.24")]
        public const string ChatModel = "deepseek-chat";

        [Obsolete("Deprecating in 2026.07.24")]
        public const string ReasonerModel = "deepseek-reasoner";

        public const int MaxContextLength = 1_000_000;
        public const int MaxOutputLength = 384_000_000;
    }
}