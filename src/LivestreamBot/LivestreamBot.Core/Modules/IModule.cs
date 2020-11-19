using SimpleInjector;

namespace LivestreamBot.Core
{
    public interface IModule
    {
        void Register(Container container);
    }
}
