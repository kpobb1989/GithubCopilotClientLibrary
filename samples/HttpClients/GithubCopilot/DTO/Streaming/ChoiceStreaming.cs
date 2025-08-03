using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming
{
    internal class ChoiceStreaming
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("delta")]
        public Message Delta { get; set; }

        [JsonProperty("finish_reason")]
        public string? FinishReason { get; set; }
    }
}
