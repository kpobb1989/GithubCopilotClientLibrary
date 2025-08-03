using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    internal partial class GithubCopilotHttpClient
    {
        // Resp

        public class ChatCompletionResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("object")]
            public string Object { get; set; } = string.Empty;

            [JsonPropertyName("created")]
            public long Created { get; set; }

            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("choices")]
            public List<ChoiceNonStreaming> Choices { get; set; } = new();

            [JsonPropertyName("system_fingerprint")]
            public string? SystemFingerprint { get; set; }

            [JsonPropertyName("usage")]
            public Usage? Usage { get; set; }
        }
    }
}
