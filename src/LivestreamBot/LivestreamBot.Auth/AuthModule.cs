using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using LivestreamBot.Core.DI;
using LivestreamBot.Core;

using SimpleInjector;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
