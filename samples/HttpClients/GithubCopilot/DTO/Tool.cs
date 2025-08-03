using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO
{
    public class Tool
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;
    }
}
