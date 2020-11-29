
using LivestreamBot.Core.DI;
using LivestreamBot.Livestream.Events;
using LivestreamBot.Livestream.Notifications;
using LivestreamBot.Livestream.Scheduling;

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
            container.Register<ISchedulingService, SchedulingService>();

            container.Collection.Register<ILivestreamTimeTriggeredEventNotificationHandler>(assemblies);

            container.Register<IYoutubeServiceProvider, YoutubeServiceProvider>();
            container.Register(() => container.GetInstance<IYoutubeServiceProvider>().GetDefaultService(), Lifestyle.Scoped);
        }
    }
}


