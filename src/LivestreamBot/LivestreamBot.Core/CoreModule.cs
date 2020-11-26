using LivestreamBot.Core.DI;
using LivestreamBot.Core.Environment;

using Microsoft.Extensions.Configuration;

using SimpleInjector;

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LivestreamBot.Core
{
    public class CoreModule : IModule
    {

        public void Register(Container container, IList<Assembly> assemblies)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", true);

            container.RegisterInstance<IConfiguration>(builder.Build());
            container.Register(TimezoneInfoProvider.GetLocalTimeZoneInfo);
            container.Register<IAppConfig, AppConfig>(Lifestyle.Singleton);
        }
    }
}
