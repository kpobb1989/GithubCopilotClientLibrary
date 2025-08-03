namespace GithubApiProxy.DTO
{
    public class GithubCopilotQuota
    {
        public int Entitlement { get; init; }

        public double PercentRemaining { get; init; }

        public double QuotaRemaining { get; init; }

        public bool Unlimited { get; init; }
    }
}
