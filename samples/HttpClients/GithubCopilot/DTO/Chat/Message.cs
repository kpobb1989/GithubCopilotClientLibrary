using Newtonsoft.Json;
using System.Collections.Generic;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat
{
    internal class Message
    {
        [JsonProperty("role")]
        public string Role { get; set; } = null!;

        [JsonProperty("content")]
        public string Content { get; set; } = null!;

        // Add support for tool_calls
        [JsonProperty("tool_calls")]
        public List<ToolCall>? ToolCalls { get; set; }

        // Add support for tool_call_id in tool responses
        [JsonProperty("tool_call_id", NullValueHandling = NullValueHandling.Ignore)]
        public string? ToolCallId { get; set; }

        public Message() { }

        public Message(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }

    public class ToolCall
    {
        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        [JsonProperty("type")]
        public string Type { get; set; } = null!;

        [JsonProperty("function")]
        public ToolCallFunction Function { get; set; } = null!;
    }

    public class ToolCallFunction
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("arguments")]
        public string Arguments { get; set; } = null!;
    }
}
