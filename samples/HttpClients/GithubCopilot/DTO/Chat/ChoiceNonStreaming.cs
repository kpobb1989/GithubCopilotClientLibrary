using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    public class ChoiceNonStreaming
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("message")]
        public Message? Message { get; set; }

        [JsonProperty("finish_reason")]
        public string? FinishReason { get; set; }
    }
}
