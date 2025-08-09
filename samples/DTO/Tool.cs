using System;
using System.Threading.Tasks;

namespace GithubApiProxy.DTO
{
    public class Tool<TParams> where TParams : class
    {
        public Type ParamType => typeof(TParams);
        public string Description { get; }
        private readonly Func<TParams, Task<object?>> _handler;
        public Tool(string description, Func<TParams, Task<object?>> handler)
        {
            Description = description;
            _handler = handler;
        }
        public Tool(string description, Func<TParams, object?> handler)
        {
            Description = description;
            _handler = (p) => Task.FromResult(handler(p));
        }
        public async Task<object?> Handle(TParams parameters)
        {
            return await _handler(parameters);
        }
    }
}
