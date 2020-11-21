using LivestreamBot.Core.Modules;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LivestreamBot.Livestream
{
    public class LivestreamModule : IModule
    {
        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.Register<IRecuringEventsProvider, RecuringEventsProvider>();
        }
    }
}
