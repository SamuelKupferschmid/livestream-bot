using LivestreamBot.Core.DI;
using LivestreamBot.Core.Environment;
using LivestreamBot.Core.Logger;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace LivestreamBot.Core
{
    public class CoreModule : IModule
    {

        public void Register(Container container, IList<Assembly> assemblies)
        {
            // Config
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var builder = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", true);

            container.RegisterInstance<IConfiguration>(builder.Build());
            container.Register<IAppConfig, AppConfig>(Lifestyle.Singleton);

            // Logging (IAsyncAwareLoggerFactory is registered in FunctionsModule itself)
            container.Register(typeof(ILogger<>), typeof(Logger<>));

            container.Register(TimezoneInfoProvider.GetLocalTimeZoneInfo);
        }
    }
}
