using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    internal partial class GithubCopilotHttpClient
    {
        public class Tool
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            // Add other properties as needed
        }
    }
}
