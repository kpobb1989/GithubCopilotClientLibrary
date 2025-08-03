using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Json
{
    internal class JsonSchema
    {
        [JsonProperty("schema")]
        public object? Schema { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
