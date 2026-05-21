using Newtonsoft.Json;

namespace DeepseekAPI.Messages
{
    public abstract class Message
    {
        [JsonProperty("content")]
        public string Content { get; private set; }
        [JsonProperty("role")]
        public abstract string Role { get; }

        protected Message(string content)
        {
            Content = content;
        }
    }
}

