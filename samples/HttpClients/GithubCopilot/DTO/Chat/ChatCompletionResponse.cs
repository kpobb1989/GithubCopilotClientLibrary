using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    public class ChatCompletionResponse
    {

        [JsonProperty("object")]
        public string? Object { get; set; }

        [JsonProperty("created")]
        public long Created { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; } = null!;

        [JsonProperty("choices")]
        public List<ChoiceNonStreaming> Choices { get; set; } = new();
    }
}
