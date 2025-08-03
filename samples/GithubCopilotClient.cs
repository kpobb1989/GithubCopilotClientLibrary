using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.Extensions;
using GithubApiProxy.HttpClients.GithubCopilot.DTO;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GithubApiProxy
{
    public class GithubCopilotClient : IGithubCopilotClient
    {
        private readonly IGithubApiHttpClient _githubApiHttpClient;
        private readonly IGithubWebHttpClient _githubWebHttpClient;
        private readonly IGithubCopilotHttpClient _githubCopilotHttpClient;

        private readonly GithubCopilotOptions _options;
        private readonly JsonSerializer _jsonSerializer;
        private readonly JSchemaGenerator _jsonSchemaGenerator = new();

        private string? githubAccessToken = null;

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
            JsonSerializer jsonSerializer,
            GithubCopilotOptions options)
        {
            _githubApiHttpClient = githubApiHttpClient;
            _githubWebHttpClient = githubWebHttpClient;
            _githubCopilotHttpClient = githubCopilotHttpClient;
            _jsonSerializer = jsonSerializer;
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
            _jsonSerializer = serviceProvider.GetRequiredService<JsonSerializer>();

            _options = options;
        }

        public void Dispose()
        {
            _githubApiHttpClient.Dispose();
            _githubCopilotHttpClient.Dispose();
        }

        public async Task AuthenticateAsync(bool force = false, CancellationToken ct = default)
        {
            if (!force && !string.IsNullOrEmpty(githubAccessToken))
            {
                return;
            }

            AccessTokenDto? accessToken = null;

            var githubTokenPath = Path.Combine(AppContext.BaseDirectory, _options.GithubTokenFileName);

            if (File.Exists(githubTokenPath))
            {
                using var fileStream = File.OpenRead(githubTokenPath);

                accessToken = _jsonSerializer.Deserialize<AccessTokenDto>(fileStream);
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

                _jsonSerializer.Serialize(fileStream!, accessToken);
            }

            _githubApiHttpClient.SetAccessToken(accessToken.AccessToken);

            githubAccessToken = accessToken.AccessToken;

            Console.WriteLine("Authentication successful. You can now use the GitHub Copilot API.");
        }

        public async Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default)
        {
            await AutoSignInAsync(ct);

            var request = GetCompletionRequest(prompt);

            var response = await _githubCopilotHttpClient.GetChatCompletionAsync(request, ct);

            var message = response.Choices?.FirstOrDefault()?.Message;

            if (_options.KeepConversationHistory && message != null)
            {
                ConversationHistory.Add(message);
            }

            return message?.Content ?? null;
        }

        public async Task<T?> GetJsonCompletionAsync<T>(string prompt, CancellationToken ct = default) where T : class
        {
            await AutoSignInAsync(ct);

            var format = new ResponseFormat
            {
                Type = "json_schema",
                JsonSchema = new JsonSchema
                {
                    Name = typeof(T).Name,
                    Schema = _jsonSchemaGenerator.Generate(typeof(T))
                }
            };

            var request = GetCompletionRequest(prompt, responseFormat: format);

            var response = await _githubCopilotHttpClient.GetChatCompletionAsync(request, ct);

            var message = response.Choices?.FirstOrDefault()?.Message;

            if (_options.KeepConversationHistory && message != null)
            {
                ConversationHistory.Add(message);
            }

            if (message?.Content == null)
            {
                return null;
            }

            return _jsonSerializer.Deserialize<T>(message.Content);
        }

        public async IAsyncEnumerable<Message?> GetChatCompletionAsync(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
        {
            await AutoSignInAsync(ct);

            var request = GetCompletionRequest(prompt, stream: true);

            var chunks = new List<Message>();

            await foreach (var chunk in _githubCopilotHttpClient.GetChatCompletionStreamingAsync(request, ct: ct))
            {
                var message = chunk?.Choices?.FirstOrDefault()?.Delta;

                if (message != null)
                {
                    chunks.Add(message);
                }

                yield return message;
            }

            if (_options.KeepConversationHistory && chunks.Count > 0)
            {
                var completeMessage = new Message(chunks[0].Role, string.Join("", chunks.Select(s => s.Content)));

                ConversationHistory.Add(completeMessage);
            }
        }

        private async Task AutoSignInAsync(CancellationToken ct = default)
        {
            if (_options.AutoSignIn)
            {
                await AuthenticateAsync(ct: ct);
            }
        }

        private ChatCompletionRequest GetCompletionRequest(string prompt, bool stream = false, ResponseFormat? responseFormat = null)
        {
            return new ChatCompletionRequest
            {
                FrequencyPenalty = _options.FrequencyPenalty,
                PresencePenalty = _options.PresencePenalty,
                Temperature = _options.Temperature,
                TopP = _options.TopP,
                ResponseFormat = responseFormat,
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
