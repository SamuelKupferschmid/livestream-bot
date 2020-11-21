using SimpleInjector;

using System.Collections.Generic;
using System.Reflection;

namespace LivestreamBot.Core.Modules
{
    public interface IModule
    {
        void Register(Container container, IList<Assembly> assemblies);
    }
}
