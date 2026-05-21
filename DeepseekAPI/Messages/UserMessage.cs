using Newtonsoft.Json;

namespace DeepseekAPI.Messages
{
    public sealed class UserMessage : Message
    {
        public const string UserRoleName = "user";
        public override string Role => UserRoleName;
        [JsonProperty("name")]
        public string? Name { get; }

        public UserMessage(string content, string? name = null)
            : base(content)
        {
            Name = name;
        }
    }
}