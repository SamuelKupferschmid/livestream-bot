using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Core.DI;
using LivestreamBot.Core.Logger;

using MediatR;

using Microsoft.Extensions.Logging;

using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace LivestreamBot.Functions
{
    public class FuncionsBase
    {
        private static readonly Container container;
        private static readonly string assemblyPrefix = "LivestreamBot.";
        private static readonly IAsyncAwareLoggerFactory loggerFactory = new AsyncAwareLoggerFactory();

        public FuncionsBase(ILoggerFactory factory)
        {
            loggerFactory.Factory = factory;
        }

        static FuncionsBase()
        {
            container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            var assemblies = Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyPrefix + "*.dll")
                .Select(filename => Assembly.Load(AssemblyName.GetAssemblyName(filename)))
                .ToList();

            container.RegisterModules(assemblies);


            container.Register<ILoggerFactory>(() => loggerFactory, Lifestyle.Singleton);

            // container.Verify();
        }

        public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                await container.GetInstance<IMediator>().Publish(notification, cancellationToken);
            }
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                return await container.GetInstance<IMediator>().Send(request, cancellationToken);
            }
        }
    }
}
