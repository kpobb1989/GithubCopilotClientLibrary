using GithubApiProxy.HttpClients.GithubApi.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GithubApiProxy.HttpClients.GithubApi
{
    internal class GithubApiHttpClient(IHttpClientFactory httpClientFactory) : IDisposable
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(nameof(GithubApiHttpClient));

        public void SetAccessToken(string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<UserDto> GetUserAsync(CancellationToken ct = default)
            => await _client.GetFromJsonAsync<UserDto>("user", ct) ?? throw new Exception($"Can not deserialize {nameof(UserDto)}");

        public async Task<CopilotTokenDto> GetCopilotTokenAsync(CancellationToken ct = default)
            => await _client.GetFromJsonAsync<CopilotTokenDto>("copilot_internal/v2/token", cancellationToken: ct) ?? throw new Exception($"Can not deserialize {nameof(CopilotTokenDto)}");

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
