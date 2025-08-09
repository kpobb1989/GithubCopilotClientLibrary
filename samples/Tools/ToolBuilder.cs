using GithubApiProxy.HttpClients.GithubCopilot.DTO.Chat;
using Newtonsoft.Json.Schema.Generation;

namespace GithubApiProxy.Tools
{
    public class ToolBuilder
    {
        private static readonly JSchemaGenerator JsonSchemaGenerator = new();
        private readonly List<Tool> _tools = [];

        public ToolBuilder AddTool<TRequest>(string toolDescription, Func<TRequest, Task<object?>> toolHandler) where TRequest : class
        {
            var type = typeof(TRequest);
            var tool = new Tool<TRequest>(
                type: "function",
                function: new ToolFunction
                {
                    Name = type.Name,
                    Description = toolDescription,
                    Parameters = JsonSchemaGenerator.Generate(type),
                    ParametersType = type
                },
                handler: toolHandler
            );
            _tools.Add(tool);

            return this;
        }

        public List<Tool> Build() => _tools;
    }
}
