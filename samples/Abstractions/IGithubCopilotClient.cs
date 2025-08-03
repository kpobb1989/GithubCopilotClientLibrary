using GithubApiProxy.DTO;

namespace GithubApiProxy.Abstractions
{
    internal interface IGithubCopilotClient : IDisposable
    {
        /// <summary>
        /// Retrieves the history of messages exchanged in the current conversation.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="GithubCopilotMessage"/> objects representing the messages in the
        /// conversation history. The collection will be empty if no messages exist in the history.</returns>
        IEnumerable<GithubCopilotMessage> GetConversationHistory();

        /// <summary>
        /// Authenticates the user asynchronously.
        /// </summary>
        /// <remarks>Use this method to authenticate a user in scenarios where authentication is required.
        /// If <paramref name="force"/> is set to <see langword="true"/>, the method ensures that the authentication
        /// process is performed regardless of any existing cached state.</remarks>
        /// <param name="force">A value indicating whether to force re-authentication.  If <see langword="true"/>, the method bypasses any
        /// cached authentication state and performs a fresh authentication. If <see langword="false"/>, cached
        /// authentication state may be used if available.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the authentication operation.</param>
        /// <returns>A task that represents the asynchronous authentication operation.</returns>
        Task AuthenticateAsync(bool force = false, CancellationToken ct = default);

        /// <summary>
        /// Generates a text completion based on the provided prompt.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to generate text based on the given
        /// prompt.  The result may vary depending on the underlying implementation or model used.</remarks>
        /// <param name="prompt">The input text used to generate the completion. This must be a non-empty string.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation. Defaults to <see langword="default"/> if not
        /// provided.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The result contains the generated text
        /// completion as a string,  or <see langword="null"/> if the operation fails or no completion is generated.</returns>
        Task<string?> GetTextCompletionAsync(string prompt, CancellationToken ct = default);

        /// <summary>
        /// Sends a prompt to a JSON-based completion service and retrieves the deserialized response as an object of
        /// type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>This method interacts with a JSON-based completion service, which is expected to
        /// return a valid JSON response.  Ensure that the type <typeparamref name="T"/> matches the structure of the
        /// expected JSON response for successful deserialization.</remarks>
        /// <typeparam name="T">The type of the object to deserialize the JSON response into. Must be a reference type.</typeparam>
        /// <param name="prompt">The input prompt to send to the completion service. Cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation. Defaults to <see
        /// cref="CancellationToken.None"/>.</param>
        /// <returns>A task representing the asynchronous operation. The result contains the deserialized object of type
        /// <typeparamref name="T"/>,  or <see langword="null"/> if the response cannot be deserialized or the service
        /// returns no data.</returns>
        Task<T?> GetJsonCompletionAsync<T>(string prompt, CancellationToken ct = default) where T : class;

        /// <summary>
        /// Generates a stream of chat completion responses based on the provided prompt.
        /// </summary>
        /// <remarks>This method returns responses incrementally as they are generated, allowing the
        /// caller to process them in real-time. Ensure proper handling of <see langword="null"/> values in the returned
        /// stream.</remarks>
        /// <param name="prompt">The input text or query to generate the chat completion for. This parameter cannot be null or empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation. Defaults to <see langword="default"/> if not
        /// provided.</param>
        /// <returns>An asynchronous stream of strings representing the chat completion responses. Each response may be <see
        /// langword="null"/> if no output is generated.</returns>
        IAsyncEnumerable<string?> GetChatCompletionAsync(string prompt, CancellationToken ct = default);

        /// <summary>
        /// Retrieves the usage statistics for GitHub Copilot.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a  <see
        /// cref="GithubCopiotUsage"/> object with the usage statistics for GitHub Copilot.</returns>
        Task<GithubCopiotUsage> GetCopilotUsageAsync(CancellationToken ct = default);
    }
}
