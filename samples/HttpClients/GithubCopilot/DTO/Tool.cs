using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO
{
    public class Tool
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;
    }
}
