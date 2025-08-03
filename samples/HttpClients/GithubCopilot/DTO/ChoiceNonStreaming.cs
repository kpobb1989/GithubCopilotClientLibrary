using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO
{
    public class ChoiceNonStreaming
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }
}
