using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO
{
    public class Message
    {
        [JsonProperty("role")]
        public string Role { get; set; } = null!;

        [JsonProperty("content")]
        public string Content { get; set; } = null!;

        public Message() { }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}
