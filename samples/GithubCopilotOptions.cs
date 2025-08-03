namespace GithubApiProxy
{
    /// <summary>
    /// Represents configuration options for GitHub Copilot integration.
    /// </summary>
    public class GithubCopilotOptions
    {
        /// <summary>
        /// Gets or sets the OAuth client ID for GitHub authentication.
        /// </summary>
        public string ClientId { get; set; } = "01ab8ac9400c4e429b23";

        /// <summary>
        /// Gets or sets the OAuth scope for GitHub authentication.
        /// </summary>
        public string Scope { get; set; } = "read:user";

        /// <summary>
        /// Gets or sets the user agent string used in HTTP requests.
        /// </summary>
        public string UserAgent { get; set; } = "VSCopilotClient/17.14.878.3237";

        /// <summary>
        /// Gets or sets the editor version string sent in HTTP headers.
        /// </summary>
        public string EditorVersion { get; set; } = "VS/VisualStudio.17.Release/17.14.36310.24";

        /// <summary>
        /// Gets or sets the GitHub API version to use in requests.
        /// </summary>
        public string GithubApiVersion { get; set; } = "2022-11-28";

        /// <summary>
        /// Gets or sets the file name used to store the GitHub token.
        /// </summary>
        public string GithubTokenFileName { get; set; } = "github_token.json";

        /// <summary>
        /// Gets or sets the default model name for Copilot completions.
        /// </summary>
        public string Model { get; set; } = "gpt-4.1";

        /// <summary>
        /// Gets or sets the frequency penalty parameter for completions.
        /// </summary>
        public int FrequencyPenalty { get; set; } = 0;

        /// <summary>
        /// Gets or sets the presence penalty parameter for completions.
        /// </summary>
        public int PresencePenalty { get; set; } = 0;

        /// <summary>
        /// Gets or sets the temperature parameter for completions.
        /// </summary>
        public int Temperature { get; set; } = 0;

        /// <summary>
        /// Gets or sets the top_p parameter for completions.
        /// </summary>
        public int TopP { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of completions to generate.
        /// </summary>
        public int N { get; set; } = 1;
    }
}
