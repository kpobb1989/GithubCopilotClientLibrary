using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using Newtonsoft.Json;
using GithubApiProxy.Extensions;

namespace GithubApiProxy.HttpClients.GithubWeb
{
    internal class GithubWebHttpClient(IHttpClientFactory httpClientFactory, JsonSerializer jsonSerializer, GithubCopilotOptions options) : IGithubWebHttpClient
    {
        public async Task<DeviceCodeResponse> GetDeviceCodeAsync(CancellationToken ct = default)
        {
            using var client = httpClientFactory.CreateClient(nameof(GithubWebHttpClient));

            var body = new Dictionary<string, string>
            {
                { "client_id", options.ClientId },
                { "scope", options.Scope }
            };

            return await client.ExecuteAndGetJsonAsync<DeviceCodeResponse>("login/device/code", HttpMethod.Post, jsonSerializer, body, ct);
        }

        public async Task<AccessTokenResponse> WaitForAccessTokenAsync(string deviceCode, int interval, CancellationToken ct = default)
        {
            using var client = httpClientFactory.CreateClient(nameof(GithubWebHttpClient));

            var delay = (interval + 1) * 1000;

            var body = new Dictionary<string, string>
            {
                { "client_id", options.ClientId },
                { "device_code", deviceCode },
                { "grant_type", "urn:ietf:params:oauth:grant-type:device_code" }
            };

            while (true)
            {
                using var responseStream = await client.ExecuteAndGetStreamAsync("login/oauth/access_token", HttpMethod.Post, jsonSerializer, body, ct);

                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream, ct);
                memoryStream.Position = 0;

                var error = jsonSerializer.Deserialize<OAuthErrorDto>(memoryStream);

                if (!string.IsNullOrEmpty(error?.Error))
                {
                    if (error.Error == "authorization_pending")
                    {
                        await Task.Delay(delay, ct);

                        continue;
                    }
                    else
                    {
                        throw new Exception($"GitHub OAuth error: {error.ErrorDescription ?? error.Error}");
                    }
                }

                memoryStream.Position = 0;

                return jsonSerializer.Deserialize<AccessTokenResponse>(memoryStream) ?? throw new Exception($"Can not deserialize {nameof(AccessTokenResponse)}"); ;
            }
        }
    }
}
