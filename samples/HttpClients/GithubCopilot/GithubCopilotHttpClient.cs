using GithubApiProxy.HttpClients.GithubApi;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    internal class GithubCopilotHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly GithubApiHttpClient _githubApiHttpClient;

        public GithubCopilotHttpClient(IHttpClientFactory httpClientFactory, GithubApiHttpClient githubApiHttpClient)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(GithubCopilotHttpClient));

            _githubApiHttpClient = githubApiHttpClient;
        }

        public async Task<ChatCompletionResponse> GetCompletionAsync(ChatCompletionsDto body, CancellationToken ct = default)
        {
            var token = await _githubApiHttpClient.GetCopilotTokenAsync(ct);

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Token);

            var response = await _httpClient.PostAsJsonAsync("chat/completions", body, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: ct)
                ?? throw new Exception("Cannot deserialize completion response");
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        public class ChatCompletionsDto
        {
            [JsonPropertyName("messages")]
            public List<Message> Messages { get; set; } = new();

            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("temperature")]
            public double? Temperature { get; set; }

            [JsonPropertyName("top_p")]
            public double? TopP { get; set; }

            [JsonPropertyName("max_tokens")]
            public int? MaxTokens { get; set; }

            [JsonPropertyName("stop")]
            public object? Stop { get; set; }

            [JsonPropertyName("n")]
            public int? N { get; set; }

            [JsonPropertyName("stream")]
            public bool? Stream { get; set; }

            [JsonPropertyName("frequency_penalty")]
            public double? FrequencyPenalty { get; set; }

            [JsonPropertyName("presence_penalty")]
            public double? PresencePenalty { get; set; }

            [JsonPropertyName("logit_bias")]
            public Dictionary<string, double>? LogitBias { get; set; }

            [JsonPropertyName("logprobs")]
            public bool? Logprobs { get; set; }

            [JsonPropertyName("response_format")]
            public ResponseFormat? ResponseFormat { get; set; }

            [JsonPropertyName("seed")]
            public int? Seed { get; set; }

            [JsonPropertyName("tools")]
            public List<Tool>? Tools { get; set; }

            [JsonPropertyName("tool_choice")]
            public object? ToolChoice { get; set; }

            [JsonPropertyName("user")]
            public string? User { get; set; }
        }

        public class Message
        {
            [JsonPropertyName("role")]
            public string Role { get; set; } = string.Empty;

            [JsonPropertyName("content")]
            public string Content { get; set; } = string.Empty;
        }

        public class Tool
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            // Add other properties as needed
        }

        public class ResponseFormat
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;
        }

        // Resp

        public class ChatCompletionResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("object")]
            public string Object { get; set; } = string.Empty;

            [JsonPropertyName("created")]
            public long Created { get; set; }

            [JsonPropertyName("model")]
            public string Model { get; set; } = string.Empty;

            [JsonPropertyName("choices")]
            public List<ChoiceNonStreaming> Choices { get; set; } = new();

            [JsonPropertyName("system_fingerprint")]
            public string? SystemFingerprint { get; set; }

            [JsonPropertyName("usage")]
            public Usage? Usage { get; set; }
        }

        public class ChoiceNonStreaming
        {
            [JsonPropertyName("index")]
            public int Index { get; set; }

            [JsonPropertyName("message")]
            public GithubCopilotHttpClient.Message? Message { get; set; }

            [JsonPropertyName("finish_reason")]
            public string? FinishReason { get; set; }

            // Add other properties if needed
        }

        public class Usage
        {
            [JsonPropertyName("prompt_tokens")]
            public int PromptTokens { get; set; }

            [JsonPropertyName("completion_tokens")]
            public int CompletionTokens { get; set; }

            [JsonPropertyName("total_tokens")]
            public int TotalTokens { get; set; }
        }
    }
}
