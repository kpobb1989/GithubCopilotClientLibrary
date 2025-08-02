namespace GithubApiProxy
{
    public static class AppSettings
    {
        public static string ClientId { get; } = "01ab8ac9400c4e429b23";
        public static string Scope { get; } = "read:user";
        public static string UserAgent { get; } = "VSCopilotClient/17.14.878.3237";
        public static string EditorVersion { get; } = "VS/VisualStudio.17.Release/17.14.36310.24";
        public static string EditorPluginVersion { get; } = "copilot-vs-chat/17.14.878.3237";
        public static string GithubApiVersion { get; } = "2022-11-28";

        public static string GithubTokenFileName { get; } = "github_token.json";
    }
}
