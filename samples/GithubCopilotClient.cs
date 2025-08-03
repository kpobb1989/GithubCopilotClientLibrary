using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.HttpClients.GithubCopilot.DTO;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace GithubApiProxy
{
    public class GithubCopilotClient : IGithubCopilotClient
    {
        private readonly IGithubApiHttpClient _githubApiHttpClient;
        private readonly IGithubWebHttpClient _githubWebHttpClient;
        private readonly IGithubCopilotHttpClient _githubCopilotHttpClient;

        private readonly GithubCopilotOptions _options;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public List<Message> ConversationHistory { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="GithubCopilotClient"/> class, providing access to GitHub API,
        /// web, and Copilot-specific HTTP clients, as well as configuration options.
        /// </summary>
        /// <remarks>This constructor is designed to initialize the client with the necessary dependencies
        /// for interacting with GitHub services. Ensure that all provided dependencies are properly configured before
        /// instantiating the client.</remarks>
        /// <param name="githubApiHttpClient">An instance of <see cref="IGithubApiHttpClient"/> used to interact with GitHub's API.</param>
        /// <param name="githubWebHttpClient">An instance of <see cref="IGithubWebHttpClient"/> used to perform web-based HTTP operations with GitHub.</param>
        /// <param name="githubCopilotHttpClient">An instance of <see cref="IGithubCopilotHttpClient"/> used to interact with GitHub Copilot-specific
        /// endpoints.</param>
        /// <param name="options">A <see cref="GithubCopilotOptions"/> object containing configuration settings for the client.</param>
        public GithubCopilotClient(
            IGithubApiHttpClient githubApiHttpClient,
            IGithubWebHttpClient githubWebHttpClient,
            IGithubCopilotHttpClient githubCopilotHttpClient,
            GithubCopilotOptions options)
        {
            _githubApiHttpClient = githubApiHttpClient;
            _githubWebHttpClient = githubWebHttpClient;
            _githubCopilotHttpClient = githubCopilotHttpClient;
            _options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GithubCopilotClient"/> class, configuring it with the specified
        /// options.
        /// </summary>
        /// <remarks>This constructor sets up the necessary services and dependencies for interacting with
        /// GitHub Copilot APIs. It uses dependency injection to initialize internal HTTP clients required for API
        /// communication.</remarks>
        /// <param name="options">The configuration options for the GitHub Copilot client. If <paramref name="options"/> is <see
        /// langword="null"/>, default options will be used.</param>
        public GithubCopilotClient(GithubCopilotOptions? options = null)
        {
            options ??= new GithubCopilotOptions();

            var services = new ServiceCollection();

            services.AddGithubCopilotModule(options);

            var serviceProvider = services.BuildServiceProvider();

            _githubApiHttpClient = serviceProvider.GetRequiredService<IGithubApiHttpClient>();
            _githubWebHttpClient = serviceProvider.GetRequiredService<IGithubWebHttpClient>();
            _githubCopilotHttpClient = serviceProvider.GetRequiredService<IGithubCopilotHttpClient>();

            _options = options;
        }

        public async Task AuthenticateAsync(CancellationToken ct = default)
        {
            AccessTokenDto? accessToken = null;

            var githubTokenPath = Path.Combine(AppContext.BaseDirectory, _options.GithubTokenFileName);

            if (File.Exists(githubTokenPath))
            {
                using var fileStream = File.OpenRead(githubTokenPath);

                accessToken = await JsonSerializer.DeserializeAsync<AccessTokenDto>(fileStream, cancellationToken: ct);
            }
            if (accessToken == null)
            {
                var deviceCode = await _githubWebHttpClient.GetDeviceCodeAsync(ct);

                Console.WriteLine($"Please enter the code {deviceCode.UserCode} in {deviceCode.VerificationUri}");

                if (_options.OpenBrowserOnAuthenticate)
                {
                    Console.WriteLine("Opening browser for authentication...");

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = deviceCode.VerificationUri,
                        UseShellExecute = true
                    });
                }

                accessToken = await _githubWebHttpClient.WaitForAccessTokenAsync(deviceCode.DeviceCode, deviceCode.Interval, ct);

                using var fileStream = File.Create(githubTokenPath);

                await JsonSerializer.SerializeAsync(fileStream, accessToken, _jsonSerializerOptions, ct);
            }

            _githubApiHttpClient.SetAccessToken(accessToken.AccessToken);
        }

        /// <summary>
        /// Releases the resources used by the current instance, including any associated HTTP clients.
        /// </summary>
        /// <remarks>This method disposes of the underlying HTTP clients used for GitHub API and Copilot
        /// interactions. After calling this method, the instance should not be used further.</remarks>
        public void Dispose()
        {
            _githubApiHttpClient.Dispose();
            _githubCopilotHttpClient.Dispose();
        }


        public async Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default)
        {
            var request = GetCompletionRequest(prompt, stream: false);

            var response = await _githubCopilotHttpClient.GetChatCompletionAsync(request, ct);

            var messge = response.Choices?.FirstOrDefault()?.Message;

            if (_options.KeepConversationHistory && messge != null)
            {
                ConversationHistory.Add(messge);
            }

            return messge?.Content ?? null;
        }

        public async IAsyncEnumerable<Message?> GetChatCompletionAsync(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var request = GetCompletionRequest(prompt, stream: true);

            await foreach (var chunk in _githubCopilotHttpClient.GetChatCompletionStreamingAsync(request, ct: ct))
            {
                yield return chunk?.Choices?.FirstOrDefault()?.Delta;
            }
        }

        private ChatCompletionRequest GetCompletionRequest(string prompt, bool stream)
        {
            return new ChatCompletionRequest
            {
                FrequencyPenalty = _options.FrequencyPenalty,
                PresencePenalty = _options.PresencePenalty,
                Temperature = _options.Temperature,
                TopP = _options.TopP,
                N = _options.N,
                Stream = stream,
                Model = _options.Model,
                Messages = GetMessages(prompt)
            };
        }

        private List<Message> GetMessages(string prompt)
        {
            var messages = _options.KeepConversationHistory ? ConversationHistory : [];

            // Ensure the system prompt is the first message if not already present
            if (!string.IsNullOrEmpty(_options.SystemPrompt) &&
                (messages.Count == 0 || !string.Equals(messages[0].Role, "system", StringComparison.OrdinalIgnoreCase)))
            {
                messages.Insert(0, new Message("system", _options.SystemPrompt));
            }

            messages.Add(new Message("user", prompt));

            return messages;
        }
    }
}
