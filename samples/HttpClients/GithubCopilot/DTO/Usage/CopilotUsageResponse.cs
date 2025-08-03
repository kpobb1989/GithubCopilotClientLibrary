using Newtonsoft.Json;

namespace GithubApiProxy.HttpClients.GithubCopilot.DTO.Usage
{
    internal class CopilotUsageResponse
    {
        [JsonProperty("access_type_sku")]
        public string AccessTypeSku { get; set; } = string.Empty;

        [JsonProperty("analytics_tracking_id")]
        public string AnalyticsTrackingId { get; set; } = string.Empty;

        [JsonProperty("assigned_date")]
        public string AssignedDate { get; set; } = string.Empty;

        [JsonProperty("can_signup_for_limited")]
        public bool CanSignupForLimited { get; set; }

        [JsonProperty("chat_enabled")]
        public bool ChatEnabled { get; set; }

        [JsonProperty("copilot_plan")]
        public string CopilotPlan { get; set; } = string.Empty;

        [JsonProperty("organization_login_list")]
        public List<object> OrganizationLoginList { get; set; } = new();

        [JsonProperty("organization_list")]
        public List<object> OrganizationList { get; set; } = new();

        [JsonProperty("quota_reset_date")]
        public string QuotaResetDate { get; set; } = string.Empty;

        [JsonProperty("quota_snapshots")]
        public QuotaSnapshots QuotaSnapshots { get; set; } = new();
    }
}
