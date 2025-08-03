namespace GithubApiProxy.DTO
{
    public class GithubCopilotModel
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
        public bool ModelPickerEnabled { get; set; }
        public GithubCopilotModelLimits? Limits { get; set; }
        public GithubCopilotModelSupports? Supports { get; set; }
        public required string Tokenizer { get; set; }
        public required bool Preview { get; set; }
        public required string Vendor { get; set; }
        public GithubCopilotModelBilling? Billing { get; set; }
    }

    public class GithubCopilotModelBilling
    {
        public bool IsPremium { get; set; }
        public double Multiplier { get; set; }
    }

    public class GithubCopilotModelLimits
    {
        public int MaxContextWindowTokens { get; set; }
        public int MaxOutputTokens { get; set; }
        public int MaxPromptTokens { get; set; }
        public GithubCopilotVisionLimits? Vision { get; set; }
    }

    public class GithubCopilotVisionLimits
    {
        public int MaxPromptImageSize { get; set; }
        public int MaxPromptImages { get; set; }
        public List<string>? SupportedMediaTypes { get; set; }
    }

    public class GithubCopilotModelSupports
    {
        public bool? ToolCalls { get; set; }
        public bool? ParallelToolCalls { get; set; }
        public bool? StructuredOutputs { get; set; }
        public bool? Streaming { get; set; }
        public bool? Vision { get; set; }
    }
}
