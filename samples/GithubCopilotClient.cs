using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubCopilot;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using System.Diagnostics;
using System.Text.Json;

namespace GithubApiProxy
{
    public class GithubCopilotClient(
        IGithubApiHttpClient githubApiHttpClient,
        IGithubWebHttpClient githubWebHttpClient,
        IGithubCopilotHttpClient githubCopilotHttpClient) : IGithubCopilotClient
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public async Task AuthenticateAsync(CancellationToken ct = default)
        {
            AccessTokenDto? accessToken = null;

            var githubTokenPath = Path.Combine(AppContext.BaseDirectory, AppSettings.GithubTokenFileName);

            if (File.Exists(githubTokenPath))
            {
                using var fileStream = File.OpenRead(githubTokenPath);

                accessToken = await JsonSerializer.DeserializeAsync<AccessTokenDto>(fileStream, cancellationToken: ct);
            }
            if (accessToken == null)
            {
                var deviceCode = await githubWebHttpClient.GetDeviceCodeAsync(ct);

                Console.WriteLine($"Please enter the code {deviceCode.UserCode} in {deviceCode.VerificationUri}");

                Process.Start(new ProcessStartInfo
                {
                    FileName = deviceCode.VerificationUri,
                    UseShellExecute = true
                });

                accessToken = await githubWebHttpClient.GetAccessTokenAsync(deviceCode.DeviceCode, deviceCode.Interval, ct);

                using var fileStream = File.Create(githubTokenPath);

                await JsonSerializer.SerializeAsync(fileStream, accessToken, _jsonSerializerOptions, ct);
            }

            githubApiHttpClient.SetAccessToken(accessToken.AccessToken);
        }

        public void Dispose()
        {
            githubApiHttpClient.Dispose();
            githubCopilotHttpClient.Dispose();
        }

        public async Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default)
        {
            var chatCompletionsDto = new ChatCompletionsDto
            {
                FrequencyPenalty = 0,
                PresencePenalty = 0,
                Temperature = 0,
                TopP = 1,
                N = 1,
                Stream = false,
                Model = AppSettings.Model,
                Messages =
                [
                    new Message
                    {
                        Role = "user",
                        Content = prompt
                    }
                ]
            };

            var response = await githubCopilotHttpClient.GetCompletionAsync(chatCompletionsDto, ct);

            return response.Choices.FirstOrDefault()?.Message?.Content ?? null;
        }
    }
}
