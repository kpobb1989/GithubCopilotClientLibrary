using GithubApiProxy.HttpClients.GithubApi;
using System.Net.Http.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot
{
    internal partial class GithubCopilotHttpClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly GithubApiHttpClient _githubApiHttpClient;

        private string? _githubCopilotToken;
        private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public GithubCopilotHttpClient(IHttpClientFactory httpClientFactory, GithubApiHttpClient githubApiHttpClient)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(GithubCopilotHttpClient));

            _githubApiHttpClient = githubApiHttpClient;
        }

        public async Task<ChatCompletionResponse> GetCompletionAsync(ChatCompletionsDto body, CancellationToken ct = default)
        {
            var token = await GetValidCopilotTokenAsync(ct);

            using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions")
            {
                Content = JsonContent.Create(body)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request, ct);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: ct)
                ?? throw new Exception("Cannot deserialize completion response");
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
                    var newToken = await _githubApiHttpClient.GetCopilotTokenAsync(ct);
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
