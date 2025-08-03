using GithubApiProxy.HttpClients.GithubCopilot;
using GithubApiProxy.HttpClients.GithubCopilot.DTO;
using System.Runtime.CompilerServices;

namespace GithubApiProxy.Abstractions
{
    public interface IGithubCopilotClient : IDisposable
    {
        /// <summary>
        /// Authenticates the application with GitHub by obtaining an access token.
        /// </summary>
        /// <remarks>This method attempts to retrieve an access token from a local file. If the token is
        /// not found or is invalid,  it initiates a device code authentication flow, prompting the user to enter a code
        /// at the provided verification URL. The access token is then stored locally for future use.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the authentication process.</param>
        /// <returns></returns>
        Task AuthenticateAsync(CancellationToken ct = default);

        /// <summary>
        /// Generates a text completion based on the provided prompt.
        /// </summary>
        /// <remarks>This method uses a predefined model and configuration to generate text completions.
        /// The result is derived from the first choice returned by the completion service.</remarks>
        /// <param name="prompt">The input text that serves as the basis for generating the completion. This should be a meaningful string
        /// that guides the completion process.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation. Defaults to <see
        /// langword="default"/> if not provided.</param>
        /// <returns>A <see cref="string"/> containing the generated text completion, or <see langword="null"/> if no completion
        /// is available.</returns>
        Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default);

        /// <summary>
        /// Generates a stream of chat completion messages based on the provided prompt.
        /// </summary>
        /// <remarks>This method returns an asynchronous stream, allowing the caller to process chat
        /// completions as they are generated. Ensure proper handling of <see langword="null"/> values in the
        /// stream.</remarks>
        /// <param name="prompt">The input text or query to generate chat completions for. This parameter cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation. Defaults to <see langword="default"/> if not
        /// provided.</param>
        /// <returns>An asynchronous stream of <see cref="Message"/> objects representing the generated chat completions. Each
        /// item in the stream may be <see langword="null"/> if no valid completion is generated.</returns>
        IAsyncEnumerable<Message?> GetChatCompletionAsync(string prompt, CancellationToken ct = default);
    }
}
