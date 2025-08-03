using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.Extensions;
using GithubApiProxy.HttpClients.GithubApi.DTO;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GithubApiProxy.HttpClients.GithubApi
{
    internal class GithubApiHttpClient(IHttpClientFactory httpClientFactory, JsonSerializer jsonSerializer) : IGithubApiHttpClient
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(nameof(GithubApiHttpClient));

        public void SetAccessToken(string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public async Task<CopilotTokenDto> GetCopilotTokenAsync(CancellationToken ct = default)
            => await _client.ExecuteAndGetJsonAsync<CopilotTokenDto>("copilot_internal/v2/token", HttpMethod.Get, jsonSerializer, ct: ct) ?? throw new Exception($"Can not deserialize {nameof(CopilotTokenDto)}");

        public async Task<CopilotUsageResponse> GetCopilonUsageAsync(CancellationToken ct = default)
            => await _client.ExecuteAndGetJsonAsync<CopilotUsageResponse>("copilot_internal/user", HttpMethod.Get, jsonSerializer, ct: ct) ?? throw new Exception($"Can not deserialize {nameof(CopilotTokenDto)}");


        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
