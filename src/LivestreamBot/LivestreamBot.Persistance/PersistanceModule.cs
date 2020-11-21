
using LivestreamBot.Core.Modules;

using Microsoft.Azure.Cosmos.Table;

using SimpleInjector;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LivestreamBot.Persistance
{
    public class PersistanceModule : IModule
    {
        public void Register(Container container, IList<Assembly> assemblies)
        {
            var entityTypes = assemblies.SelectMany(ass => ass.GetTypes()).Where(t => typeof(TableEntity).IsAssignableFrom(t));

            foreach (var type in entityTypes)
            {
                container.Register(typeof(ITableStorage<>).MakeGenericType(type), typeof(TableStorage<>).MakeGenericType(type));
            }

        }
    }
}
