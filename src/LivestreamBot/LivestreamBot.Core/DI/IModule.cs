
using SimpleInjector;

using System.Collections.Generic;
using System.Reflection;

namespace LivestreamBot.Core.DI
{
    public interface IModule
    {
        void Register(Container container, IList<Assembly> assemblies);
    }
}
