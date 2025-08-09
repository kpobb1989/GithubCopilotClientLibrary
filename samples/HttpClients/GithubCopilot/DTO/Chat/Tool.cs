using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    public class Tool
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("function", NullValueHandling = NullValueHandling.Ignore)]
        public ToolFunction? Function { get; set; }

        // Add ToolChoice property (optional, nullable)
        [JsonProperty("tool_choice", NullValueHandling = NullValueHandling.Ignore)]
        public string? ToolChoice { get; set; }

        // Indicates if this tool can be called in parallel with others
        [JsonProperty("allow_parallel", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AllowParallel { get; set; }
    }

    public class ToolFunction
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public object? Parameters { get; set; } // Should be a JSON schema or object

        // Add ParametersType for runtime type resolution
        [Newtonsoft.Json.JsonIgnore]
        public System.Type? ParametersType { get; set; }
    }
}
