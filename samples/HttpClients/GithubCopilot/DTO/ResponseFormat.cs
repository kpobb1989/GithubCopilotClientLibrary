using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    public class ResponseFormat
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
