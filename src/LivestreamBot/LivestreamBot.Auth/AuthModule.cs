using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

using LivestreamBot.Core.DI;

using SimpleInjector;
using System.Collections.Generic;
using System.Reflection;
using LivestreamBot.Auth.Google;
using LivestreamBot.Core.Environment;

namespace LivestreamBot.Auth
{
    public class AuthModule : IModule
    {
        public void Register(Container container, IList<Assembly> assemblies)
        {
            container.Register<IDataStore, TokenResponseStore>();
            container.Register<IYouTubeAuthorizationService, YouTubeAuthorizationService>();

            container.Register(() =>
            {
                var appConfig = container.GetInstance<IAppConfig>();
                return new ClientSecrets
                {
                    ClientId = appConfig.GoogleApiClientId,
                    ClientSecret = appConfig.GoogleApiClientSecret
                };
            });
        }
    }
}
