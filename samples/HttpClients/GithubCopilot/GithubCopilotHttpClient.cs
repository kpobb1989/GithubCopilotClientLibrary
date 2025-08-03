using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.Extensions;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Streaming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    internal class GithubCopilotHttpClient(IHttpClientFactory httpClientFactory, IGithubApiHttpClient githubApiHttpClient, JsonSerializer jsonSerializer) : IGithubCopilotHttpClient
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient(nameof(GithubCopilotHttpClient));

        private string? _githubCopilotToken;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public async Task<ChatCompletionResponse> GetChatCompletionAsync(ChatCompletionRequest request, CancellationToken ct = default)
        {
            var token = await GetValidCopilotTokenAsync(ct);

            _httpClient.AddOrReplaceHeader("Authorization", $"Bearer {token}");

            return await _httpClient.ExecuteAndGetJsonAsync<ChatCompletionResponse>("chat/completions", HttpMethod.Post, jsonSerializer, request, ct);
        }

        public async IAsyncEnumerable<ChatCompletionStreamingResponse> GetChatCompletionStreamingAsync(ChatCompletionRequest request, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var token = await GetValidCopilotTokenAsync(ct);

            _httpClient.AddOrReplaceHeader("Authorization", $"Bearer {token}");

            if (request.Stream)
            {
                using var stream = await _httpClient.ExecuteAndGetStreamAsync("chat/completions", HttpMethod.Post, jsonSerializer, request, ct);

                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    var line = await reader.ReadLineAsync(ct);

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Only process lines starting with "data: "
                    if (!line.StartsWith("data: ")) continue;

                    var json = line.Substring("data: ".Length).Trim();

                    // Skip [DONE] or other non-JSON data
                    if (json == "[DONE]") continue;

                    var response = jsonSerializer.Deserialize<ChatCompletionStreamingResponse>(json);

                    if (response != null)
                    {
                        yield return response;
                    }
                }
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private async Task<string> GetValidCopilotTokenAsync(CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);
            try
            {
                // Refresh if token missing or about to expire (buffer: 60s)
                if (_githubCopilotToken == null || DateTimeOffset.UtcNow.AddSeconds(60) >= _expiresAt)
                {
                    var newToken = await githubApiHttpClient.GetCopilotTokenAsync(ct);
                    _githubCopilotToken = newToken.Token;
                    _expiresAt = DateTimeOffset.FromUnixTimeSeconds(newToken.ExpiresAt);
                }
                return _githubCopilotToken;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
