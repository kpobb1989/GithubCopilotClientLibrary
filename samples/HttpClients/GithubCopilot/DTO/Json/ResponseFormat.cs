using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Json
{
    public class ResponseFormat
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("json_schema")]
        public JsonSchema? JsonSchema { get; set; }
    }
}
