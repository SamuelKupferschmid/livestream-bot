using LivestreamBot.Core.DI;
using LivestreamBot.Livestream.Events;

using SimpleInjector;

using System.Collections.Generic;
using System.Reflection;

namespace LivestreamBot.Livestream
{
    public class LivestreamModule : IModule
    {
        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.Register<ILivestreamEventProvider, LivestreamEventProvider>();
        }
    }
}
