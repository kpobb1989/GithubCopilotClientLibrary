namespace GithubApiProxy.DTO
{
    public class GithubCopiotUsage
    {
        public string CopilotPlan { get; init; } = null!;

        public string QuotaResetDate { get; init; } = null!;

        public GithubCopilotQuota Chat { get; init; } = null!;

        public GithubCopilotQuota Completions { get; init; } = null!;

        public GithubCopilotQuota Premium { get; init; } = null!;

    }
}
