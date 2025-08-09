using System;
using System.Threading.Tasks;

namespace GithubApiProxy.DTO
{
    public enum ToolChoice
    {
        Auto,
        Required,
        None
    }

    public class GithubCopilotTool
    {
        public string Type { get; set; } = null!;
        public GithubCopilotToolFunction? Function { get; set; }
        public ToolChoice ToolChoice { get; set; } = ToolChoice.Auto;
        public bool AllowParallel { get; set; } = false;
    }

    public class GithubCopilotToolFunction
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Type? ParametersType { get; set; } // Used for schema generation
        public object? Parameters { get; set; } // Used for runtime values
    }

    public delegate Task<object?> GithubCopilotToolHandler(object? parameters);
}
