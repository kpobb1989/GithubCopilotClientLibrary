using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Json
{
    internal class ResponseFormat
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("json_schema")]
        public JsonSchema? JsonSchema { get; set; }
    }
}
