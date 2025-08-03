using GithubApiProxy.HttpClients.GithubCopilot.DTO.Json;
using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    public class ChatCompletionRequest
    {
        [JsonProperty("messages")]
        public IEnumerable<Message> Messages { get; set; } = [];

        [JsonProperty("model")]
        public string Model { get; set; } = null!;

        [JsonProperty("temperature")]
        public double? Temperature { get; set; }

        [JsonProperty("top_p")]
        public double? TopP { get; set; }

        [JsonProperty("max_tokens")]
        public int? MaxTokens { get; set; }

        [JsonProperty("stop")]
        public object? Stop { get; set; }

        [JsonProperty("n")]
        public int? N { get; set; }

        [JsonProperty("stream")]
        public bool Stream { get; set; }

        [JsonProperty("frequency_penalty")]
        public double? FrequencyPenalty { get; set; }

        [JsonProperty("presence_penalty")]
        public double? PresencePenalty { get; set; }

        [JsonProperty("logit_bias")]
        public Dictionary<string, double>? LogitBias { get; set; }

        [JsonProperty("logprobs")]
        public bool? Logprobs { get; set; }

        [JsonProperty("response_format")]
        public ResponseFormat? ResponseFormat { get; set; }

        [JsonProperty("seed")]
        public int? Seed { get; set; }

        [JsonProperty("tools")]
        public List<Tool>? Tools { get; set; }

        [JsonProperty("tool_choice")]
        public object? ToolChoice { get; set; }

        [JsonProperty("user")]
        public string? User { get; set; }
    }
}
