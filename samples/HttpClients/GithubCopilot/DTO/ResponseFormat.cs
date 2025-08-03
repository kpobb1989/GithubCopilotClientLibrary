using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO
{
    public class ResponseFormat
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("json_schema")]
        public JsonSchema? JsonSchema { get; set; }
    }

    public class JsonSchema
    {
        [JsonProperty("schema")]
        public object? Schema { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
