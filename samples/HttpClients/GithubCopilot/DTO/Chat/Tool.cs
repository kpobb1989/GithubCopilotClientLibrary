using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    // Generic Tool class with non-serialized handler
    public class Tool<TRequest> : Tool
    {
        [JsonIgnore]
        public Func<TRequest, Task<object?>>? ToolHandler { get; set; }

        public Tool(string type, ToolFunction? function = null, Func<TRequest, Task<object?>>? handler = null)
        {
            Type = type;
            Function = function;
            ToolHandler = handler;
        }
    }

    public class Tool
    {
        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("function", NullValueHandling = NullValueHandling.Ignore)]
        public ToolFunction? Function { get; set; }
    }

    public class ToolFunction
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string? Description { get; set; }

        [JsonProperty("parameters", NullValueHandling = NullValueHandling.Ignore)]
        public object? Parameters { get; set; } // Should be a JSON schema or object

        [JsonIgnore]
        public Type? ParametersType { get; set; }
    }
}
