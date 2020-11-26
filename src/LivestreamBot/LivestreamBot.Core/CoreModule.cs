using LivestreamBot.Core.DI;
using LivestreamBot.Core.Environment;

using SimpleInjector;

using System.Collections.Generic;
using System.Reflection;

namespace LivestreamBot.Core
{
    public class CoreModule : IModule
    {

        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.Register(TimezoneInfoProvider.GetLocalTimeZoneInfo);
            container.Register<IAppConfig, AppConfig>();
        }
    }
}
