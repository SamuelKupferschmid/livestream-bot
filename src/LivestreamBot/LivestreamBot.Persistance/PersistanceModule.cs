using LivestreamBot.Core;

using Microsoft.Azure.Cosmos.Table;

using SimpleInjector;

using System.Linq;

namespace LivestreamBot.Persistance
{
    public class PersistanceModule : IModule
    {
        public void Register(Container container)
        {
            var entityTypes = typeof(PersistanceModule).Assembly.GetTypes().Where(t => typeof(TableEntity).IsAssignableFrom(t));

            foreach (var type in entityTypes)
            {
                container.Register(typeof(ITableStorage<>).MakeGenericType(type), typeof(TableStorage<>).MakeGenericType(type));
            }

        }
    }
}
