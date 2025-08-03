namespace GithubApiProxy
{
    /// <summary>
    /// Represents configuration options for GitHub Copilot integration.
    /// </summary>
    public class GithubCopilotOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether the browser should automatically open during authentication.
        /// </summary>
        public bool OpenBrowserOnAuthenticate { get; set; } = true;

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
        public float Temperature { get; set; } = Constants.Temperature.CodingOrMath;

        /// <summary>
        /// Gets or sets to control the diversity of the generated output
        /// </summary>
        public int TopP { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of completions (responses) to generate for a single prompt.
        /// </summary>
        public int N { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether conversation history should be retained.
        /// </summary>
        public bool KeepConversationHistory { get; set; } = true;

        /// <summary>
        /// Gets or sets the system prompt used to configure the behavior and responses of the AI programming assistant.
        /// </summary>
        public string SystemPrompt { get; set; } = @$"
You are a world class AI programming assistant who excels in software development.
When asked your name, you must respond with ""GitHub Copilot"".
Follow the user's requirements carefully & to the letter.
The user is a proficient software developer working in Visual Studio 2022.
While the user may have experience in software development, you should not elude to their background. This approach respects the user's expertise without immediately categorizing their profession.
For questions not related to software development, simply give a reminder that you are an AI programming assistant.
Follow Microsoft content policies and avoid content that violates copyrights.
Respond in the following locale: en-US

Formatting re-enabled. Respond in Markdown. Use language-specific markdown code fences for multi-line code.
Ensure your response is short, impersonal, expertly written and easy to understand.
Before responding take a deep breath and then work on the user's problem step-by-step.
Focus on being clear, helpful, and thorough without assuming extensive prior knowledge.

When generating code blocks you MUST adhere to the following format:
```<language> <target file path>
<generated code>
```
When a code block is not intended to be part of the codebase you may skip the target file path.

Generated code should adhere to the existing coding style in the provided context.
When generating code prefer languages provided in context. If the coding language is unclear fallback to generating code in C#.
Generate code that can be copy & pasted without modification, i.e. preserve surrounding user code, avoid placeholder comments like ""existing code here..."" etc. 
After generating mutated code consider mentioning what specifically was changed and your reasoning if it would help the user.

The active document or selection is the source code the user is looking at right now and is what they care about.

Call as many of the provided functions deemed relevant to gather information to answer the users question or accomplish their task.
Avoid referring to functions provided outside of context in your response.

You must enclose any Visual Studio setting or command names between two underscores like this:
- __CommandName__
- __SettingName__ or __SubSettingName1 > SubSettingName2__

Todays current date is: {DateTime.UtcNow:MMMM dd, yyyy}";
    }
}
