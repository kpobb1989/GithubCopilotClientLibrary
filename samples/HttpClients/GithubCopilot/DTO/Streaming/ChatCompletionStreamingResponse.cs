using Newtonsoft.Json;


namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming
{

    public class ChatCompletionStreamingResponse
    {
        [JsonProperty("choices")]
        public List<ChoiceStreaming> Choices { get; set; } = [];
    }
}
