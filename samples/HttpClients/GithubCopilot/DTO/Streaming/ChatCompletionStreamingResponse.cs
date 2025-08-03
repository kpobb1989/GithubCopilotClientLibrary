using Newtonsoft.Json;


namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming
{
    internal class ChatCompletionStreamingResponse
    {
        [JsonProperty("choices")]
        public List<ChoiceStreaming> Choices { get; set; } = [];
    }
}
