using GithubApiProxy.Abstractions;
using GithubApiProxy.Abstractions.HttpClients;
using GithubApiProxy.DTO;
using GithubApiProxy.Extensions;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Json;
using GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage;
using GithubApiProxy.HttpClients.GithubWeb.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema.Generation;
using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace GithubApiProxy
{
    public class GithubCopilotClient : IGithubCopilotClient
    {
        private List<Message> ConversationHistory { get; set; } = [];

        private readonly IGithubApiHttpClient _githubApiHttpClient;
        private readonly IGithubWebHttpClient _githubWebHttpClient;
        private readonly IGithubCopilotHttpClient _githubCopilotHttpClient;
        private readonly ILogger<GithubCopilotClient> _logger;

        private readonly GithubCopilotOptions _options;
        private readonly JsonSerializer _jsonSerializer;
        private readonly JSchemaGenerator _jsonSchemaGenerator = new();

        private string? githubAccessToken = null;

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
            _logger = serviceProvider.GetRequiredService<ILogger<GithubCopilotClient>>();
            _options = options;
        }

        internal GithubCopilotClient(
            IGithubApiHttpClient githubApiHttpClient,
            IGithubWebHttpClient githubWebHttpClient,
            IGithubCopilotHttpClient githubCopilotHttpClient,
            ILogger<GithubCopilotClient> logger,
            JsonSerializer jsonSerializer,
            GithubCopilotOptions options)
        {
            _githubApiHttpClient = githubApiHttpClient;
            _githubWebHttpClient = githubWebHttpClient;
            _githubCopilotHttpClient = githubCopilotHttpClient;
            _jsonSerializer = jsonSerializer;
            _logger = logger;
            _options = options;
        }

        public void Dispose()
        {
            _githubApiHttpClient.Dispose();
            _githubCopilotHttpClient.Dispose();
        }

        public IEnumerable<GithubCopilotMessage> GetConversationHistory()
            => ConversationHistory.Select(s => new GithubCopilotMessage() { Role = s.Role, Content = s.Content });

        public async Task AuthenticateAsync(bool force = false, CancellationToken ct = default)
        {
            if (!force && !string.IsNullOrEmpty(githubAccessToken))
            {
                return;
            }

            AccessTokenResponse? accessToken = null;

            var githubTokenPath = Path.Combine(AppContext.BaseDirectory, _options.GithubTokenFileName);

            if (File.Exists(githubTokenPath))
            {
                using var fileStream = File.OpenRead(githubTokenPath);

                accessToken = _jsonSerializer.Deserialize<AccessTokenResponse>(fileStream);
            }
            if (accessToken == null)
            {
                var deviceCode = await _githubWebHttpClient.GetDeviceCodeAsync(ct);

                _logger.LogInformation($"Please enter the code {deviceCode.UserCode} in {deviceCode.VerificationUri}");

                if (_options.OpenBrowserOnAuthenticate)
                {
                    _logger.LogInformation("Opening browser for authentication...");

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

            _logger.LogInformation("Authentication successful. You can now use the GitHub Copilot API.");
        }

        public async Task<GithubCopiotUsage> GetCopilotUsageAsync(CancellationToken ct = default)
        {
            await AutoSignInAsync(ct);

            var response = await _githubApiHttpClient.GetCopilonUsageAsync(ct);

            return new GithubCopiotUsage
            {
                CopilotPlan = response.CopilotPlan,
                QuotaResetDate = response.QuotaResetDate,
                Chat = MapQuotaDetail(response.QuotaSnapshots.Chat),
                Completions = MapQuotaDetail(response.QuotaSnapshots.Completions),
                Premium = MapQuotaDetail(response.QuotaSnapshots.PremiumInteractions)
            };
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

            // Only return message content, no tool call handling here
            return message?.Content ?? null;
        }

        public async Task<string?> GetTextCompletionAsync<TRequest>(string prompt, List<Tool> tools, CancellationToken ct = default) where TRequest : class
        {
            await AutoSignInAsync(ct);

            // Step 1: Initial request with tools
            var request = GetCompletionRequest(prompt, tools, toolChoice: "required");
            var response = await _githubCopilotHttpClient.GetChatCompletionAsync(request, ct);
            var message = response.Choices?.FirstOrDefault()?.Message;
            if (_options.KeepConversationHistory && message != null)
                ConversationHistory.Add(message);

            // If no tool calls, return the assistant's message
            if (message?.ToolCalls == null || message.ToolCalls.Count == 0)
                return message?.Content ?? null;

            // Step 2: For each tool_call, invoke the handler and send the tool result back
            var followUpMessages = new List<Message>
            {
                new Message("user", prompt),
                message // assistant's tool_calls
            };

            foreach (var toolCall in message.ToolCalls)
            {
                var matchingTool = tools
                    .OfType<HttpClients.GithubCopilot.DTO.Chat.Tool<TRequest>>()
                    .FirstOrDefault(t => t.Function?.Name == toolCall.Function.Name);
                if (matchingTool?.ToolHandler == null)
                    throw new InvalidOperationException($"No handler found for tool: {toolCall.Function.Name}");
                var toolRequest = _jsonSerializer.Deserialize<TRequest>(toolCall.Function.Arguments);
                var toolResponse = await matchingTool.ToolHandler(toolRequest!);
                followUpMessages.Add(new Message("tool", JsonConvert.SerializeObject(toolResponse))
                {
                    ToolCallId = toolCall.Id
                });
            }

            // Step 3: Send follow-up request with tool result(s)
            var followUpRequest = GetCompletionRequest(null, tools); // No new prompt, just continue conversation
            followUpRequest.Messages = followUpMessages;
            var followUpResponse = await _githubCopilotHttpClient.GetChatCompletionAsync(followUpRequest, ct);
            var finalMessage = followUpResponse.Choices?.FirstOrDefault()?.Message;
            if (_options.KeepConversationHistory && finalMessage != null)
                ConversationHistory.Add(finalMessage);
            return finalMessage?.Content ?? null;
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

        public async IAsyncEnumerable<string?> GetChatCompletionAsync(string prompt, [EnumeratorCancellation] CancellationToken ct = default)
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

                yield return message?.Content;
            }

            if (_options.KeepConversationHistory && chunks.Count > 0)
            {
                var completeMessage = new Message(chunks[0].Role, string.Join("", chunks.Select(s => s.Content)));

                ConversationHistory.Add(completeMessage);
            }
        }

        public async Task<IEnumerable<GithubCopilotModel>> GetModelsAsync(CancellationToken ct = default)
        {
            await AutoSignInAsync(ct);

            var response = await _githubCopilotHttpClient.GetModelsAsync(ct);

            var models = response.Data?.Select(m =>
            {
                if (m.Name is null || m.Version is null || m.Capabilities?.Tokenizer is null || m.Preview is null || m.Vendor is null)
                    throw new InvalidOperationException("Model data is missing required fields.");

                return new GithubCopilotModel
                {
                    Name = m.Name,
                    Version = m.Version,
                    ModelPickerEnabled = m.ModelPickerEnabled,
                    Billing = m.Billing is not null ? new GithubCopilotModelBilling
                    {
                        IsPremium = m.Billing.IsPremium ?? false,
                        Multiplier = m.Billing.Multiplier ?? 0
                    } : null,
                    Limits = m.Capabilities.Limits is not null
                        ? new GithubCopilotModelLimits
                        {
                            MaxContextWindowTokens = m.Capabilities.Limits.MaxContextWindowTokens ?? 0,
                            MaxOutputTokens = m.Capabilities.Limits.MaxOutputTokens ?? 0,
                            MaxPromptTokens = m.Capabilities.Limits.MaxPromptTokens ?? 0,
                            Vision = m.Capabilities.Limits.Vision is not null
                                ? new GithubCopilotVisionLimits
                                {
                                    MaxPromptImageSize = m.Capabilities.Limits.Vision.MaxPromptImageSize ?? 0,
                                    MaxPromptImages = m.Capabilities.Limits.Vision.MaxPromptImages ?? 0,
                                    SupportedMediaTypes = m.Capabilities.Limits.Vision.SupportedMediaTypes ?? new List<string>()
                                }
                                : null
                        }
                        : null,
                    Supports = m.Capabilities.Supports is not null
                        ? new GithubCopilotModelSupports
                        {
                            ToolCalls = m.Capabilities.Supports.ToolCalls,
                            ParallelToolCalls = m.Capabilities.Supports.ParallelToolCalls,
                            StructuredOutputs = m.Capabilities.Supports.StructuredOutputs,
                            Streaming = m.Capabilities.Supports.Streaming,
                            Vision = m.Capabilities.Supports.Vision
                        }
                        : null,
                    Tokenizer = m.Capabilities.Tokenizer,
                    Preview = m.Preview.Value,
                    Vendor = m.Vendor
                };
            }).ToList() ?? new List<GithubCopilotModel>();

            return models;
        }

        private async Task AutoSignInAsync(CancellationToken ct = default)
        {
            if (_options.AutoSignIn)
            {
                await AuthenticateAsync(ct: ct);
            }
        }

        private static GithubCopilotQuota MapQuotaDetail(QuotaDetail detail)
        {
            return new GithubCopilotQuota
            {
                Entitlement = detail.Entitlement,
                PercentRemaining = detail.PercentRemaining,
                QuotaRemaining = detail.QuotaRemaining,
                Unlimited = detail.Unlimited
            };
        }

        private ChatCompletionRequest GetCompletionRequest(string? prompt = null, List<Tool>? tools = null, bool stream = false, ResponseFormat? responseFormat = null, object? toolChoice = null)
        {
            var messages = _options.KeepConversationHistory ? ConversationHistory : [];

            // Ensure the system prompt is the first message if not already present
            if (!string.IsNullOrEmpty(_options.SystemPrompt) &&
                (messages.Count == 0 || !string.Equals(messages[0].Role, "system", StringComparison.OrdinalIgnoreCase)))
            {
                messages.Insert(0, new Message("system", _options.SystemPrompt));
            }

            if (!string.IsNullOrEmpty(prompt))
            {
                messages.Add(new Message("user", prompt));
            }

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
                Messages = messages,
                Tools = tools,
                ToolChoice = toolChoice
            };
        }

        private async Task<string?> HandleToolCallAsync<TRequest>(List<Tool> tools, List<ToolCall>? toolCalls, CancellationToken ct) where TRequest : class
        {
            if (toolCalls == null || toolCalls.Count == 0)
                return null;

            var messages = _options.KeepConversationHistory ? ConversationHistory : new List<Message>();

            foreach (var toolCall in toolCalls)
            {
                var matchingTool = tools
                    .OfType<HttpClients.GithubCopilot.DTO.Chat.Tool<TRequest>>()
                    .FirstOrDefault(t => t.Function?.Name == toolCall.Function.Name);

                if (matchingTool?.ToolHandler == null)
                    throw new InvalidOperationException($"No handler found for tool: {toolCall.Function.Name}");

                var toolRequest = _jsonSerializer.Deserialize<TRequest>(toolCall.Function.Arguments);

                if (toolRequest != null)
                {
                    var toolResponse = await matchingTool.ToolHandler(toolRequest);

                    messages.Add(new Message("tool", JsonConvert.SerializeObject(toolResponse))
                    {
                        ToolCallId = toolCall.Id
                    });
                }
            }
            var request = GetCompletionRequest();
            request.Messages = messages;

            var response = await _githubCopilotHttpClient.GetChatCompletionAsync(request, ct);

            var followUpMessage = response.Choices?.FirstOrDefault()?.Message;

            if (_options.KeepConversationHistory && followUpMessage != null)
                ConversationHistory.Add(followUpMessage);

            return followUpMessage?.Content ?? null;
        }
    }
}
