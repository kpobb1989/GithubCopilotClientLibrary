using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming
{
    public class ChoiceStreaming
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("delta")]
        public Message Delta { get; set; }

        [JsonPropertyName("finish_reason")]
        public string? FinishReason { get; set; }
    }
}
