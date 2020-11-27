
using MediatR;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Mediator.Pipeline
{
    public class GenericPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<GenericPipelineBehavior<TRequest, TResponse>> logger;

        public GenericPipelineBehavior(ILogger<GenericPipelineBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            using (this.logger.BeginScope(request))
            {
                var typeName = request.GetType().Name;
                this.logger.LogInformation("Start Request of Type {type}", typeName);
                var sw = Stopwatch.StartNew();
                var response = await next();
                this.logger.LogInformation("Request of Type {type} handled in {ms}ms", typeName, sw.ElapsedMilliseconds);
                return response;
            }
        }
    }
}
