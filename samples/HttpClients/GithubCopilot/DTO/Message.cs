﻿using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
}
