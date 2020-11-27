using Google.Apis.YouTube.v3;

using LivestreamBot.Auth.Google;
using LivestreamBot.Core.Environment;

using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Livestream
{
    public interface IYoutubeServiceProvider
    {
        Task<YouTubeService> GetService(long chatId, CancellationToken cancellationToken);

        YouTubeService GetDefaultService();
    }
    public class YoutubeServiceProvider : IYoutubeServiceProvider
    {
        private readonly IAppConfig appConfig;
        private readonly IYouTubeAuthorizationService authService;

        public YoutubeServiceProvider(IAppConfig appConfig, IYouTubeAuthorizationService authService)
        {
            this.appConfig = appConfig;
            this.authService = authService;
        }

        public YouTubeService GetDefaultService()
        {
            return new YouTubeService(new YouTubeService.Initializer
            {
                ApiKey = this.appConfig.YoutubeApiKey
            });
        }

        /// <summary>
        /// Returns a Service with the correct Scope and Credentials for given ChatId
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<YouTubeService> GetService(long chatId, CancellationToken cancellationToken)
        {
            var credentials = await authService.GetChatCredentials(chatId, AuthorizationScope.Fullaccess, cancellationToken);
            if (credentials == null)
            {
                return GetDefaultService();
            }

            return new YouTubeService(new YouTubeService.Initializer
            {
                HttpClientInitializer = credentials
            });

        }
    }
}
