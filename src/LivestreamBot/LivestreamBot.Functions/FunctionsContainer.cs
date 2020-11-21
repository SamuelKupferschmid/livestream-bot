using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using LivestreamBot.Core.Modules;

using MediatR;

using Microsoft.Extensions.Configuration;

using SimpleInjector;

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

            container.Verify();
        }


        public static IMediator Mediator => container.GetInstance<IMediator>();
    }


    public class FunctionsModule : IModule
    {
        public void Register(Container container, IList<Assembly> assemblies)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("local.settings.json", true);

            container.RegisterInstance<IConfiguration>(builder.Build());
        }
    }
}
