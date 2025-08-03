using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming
{

    public class ChatCompletionStreamingResponse
    {
        [JsonPropertyName("choices")]
        public List<ChoiceStreaming> Choices { get; set; } = [];
    }
}
