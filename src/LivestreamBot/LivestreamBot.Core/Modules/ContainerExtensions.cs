using SimpleInjector;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LivestreamBot.Core
{
    public static class ContainerExtensions
    {
        public static Container RegisterModules(this Container container, IEnumerable<Assembly> assemblies)
        {
            var types = assemblies.SelectMany(ass => ass.GetTypes()).Where(type => type.IsClass && typeof(IModule).IsAssignableFrom(type));
            foreach (var type in types)
            {
                var module = Activator.CreateInstance(type) as IModule;
                module.Register(container);
            }

            return container;
        }
    }
}
