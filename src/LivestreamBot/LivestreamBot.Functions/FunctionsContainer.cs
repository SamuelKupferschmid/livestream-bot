using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Core;

using Microsoft.Extensions.Configuration;

using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace LivestreamBot.Functions
{
    public static class FunctionsContainer
    {
        private static readonly Container container;
        private static readonly string assemblyPrefix = "LivestreamBot.";

        static FunctionsContainer()
        {
            container = new Container();

            var assemblies = Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), assemblyPrefix + "*.dll")
                .Select(filename => Assembly.Load(AssemblyName.GetAssemblyName(filename)))
                .ToList();

            container.RegisterModules(assemblies);
            container.Collection.Register(typeof(IHandler<>), assemblies);            

            container.Verify();
        }

        public static async Task<bool> Handle<TEvent>(TEvent @event, CancellationToken cancellationToken)
        {
            await using (AsyncScopedLifestyle.BeginScope(container))
            {
                var handlers = container.GetAllInstances<IHandler<TEvent>>();

                foreach (var handler in handlers)
                {
                    await handler.Handle(@event, cancellationToken);
                }

                return handlers.Any();
            }
        }
    }


    public class FunctionsModule : IModule
    {
        public void Register(Container container)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("local.settings.json", true);

            container.RegisterInstance<IConfiguration>(builder.Build());
        }
    }
}
