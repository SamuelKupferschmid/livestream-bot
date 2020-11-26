using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using LivestreamBot.Core.DI;
using LivestreamBot.Core.Environment;
using LivestreamBot.Livestream.Events;
using LivestreamBot.Livestream.Notifications;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace LivestreamBot.Livestream
{
    public class LivestreamModule : IModule
    {
        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.Register<ILivestreamEventProvider, LivestreamEventProvider>();
            container.Collection.Register<ILivestreamTimeTriggeredEventNotificationHandler>(assemblies);

            container.Register(() => new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = container.GetInstance<IAppConfig>().YoutubeApiKey
            }), Lifestyle.Scoped);
        }
    }
}


