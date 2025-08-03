using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        public Message() { }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}
