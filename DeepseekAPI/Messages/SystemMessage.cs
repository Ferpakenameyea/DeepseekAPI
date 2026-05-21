using Newtonsoft.Json;

namespace DeepseekAPI.Messages
{
    public sealed class SystemMessage : Message
    {
        public const string SystemRoleName = "system";
        public override string Role => SystemRoleName;
        [JsonProperty("name")]
        public string? Name { get; }

        public SystemMessage(
            string content,
            string? name = null
        ) : base(content)
        {
            Name = name;
        }
    }
}