using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    internal class Tool
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;
    }
}
