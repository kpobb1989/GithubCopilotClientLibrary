using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using System.Net.Http.Json;
using System.Text.Json;

namespace GithubApiProxy.HttpClients.GithubWeb
{
    internal class GithubWebHttpClient(IHttpClientFactory httpClientFactory, GithubCopilotOptions options) : IGithubWebHttpClient
    {
        public async Task<DeviceCodeDto> GetDeviceCodeAsync(CancellationToken ct = default)
        {
            using var client = httpClientFactory.CreateClient(nameof(GithubWebHttpClient));

            var body = new Dictionary<string, string>
            {
                { "client_id", options.ClientId },
                { "scope", options.Scope }
            };

            var httpResponse = await client.PostAsJsonAsync("login/device/code", body, ct);

            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadFromJsonAsync<DeviceCodeDto>(ct) ?? throw new Exception($"Can not deserialize {nameof(DeviceCodeDto)}");
        }

        public async Task<AccessTokenDto> GetAccessTokenAsync(string deviceCode, int interval, CancellationToken ct = default)
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
                var httpResponse = await client.PostAsJsonAsync("login/oauth/access_token", body, ct);

                using var responseStream = await httpResponse.Content.ReadAsStreamAsync(ct);

                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream, ct);
                memoryStream.Position = 0;

                var error = await JsonSerializer.DeserializeAsync<OAuthErrorDto>(memoryStream, cancellationToken: ct);

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
                return await JsonSerializer.DeserializeAsync<AccessTokenDto>(memoryStream, cancellationToken: ct) ?? throw new Exception($"Can not deserialize {nameof(AccessTokenDto)}"); ;
            }
        }

    }
}
