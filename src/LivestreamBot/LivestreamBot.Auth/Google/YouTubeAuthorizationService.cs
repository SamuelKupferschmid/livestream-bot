using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;

using LivestreamBot.Core.Environment;
using LivestreamBot.Persistance;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Auth.Google
{
    public interface IYouTubeAuthorizationService
    {
        Task CreateChatCredentials(long chatId, TokenResponse token, AuthorizationScope scope, CancellationToken cancellationToken);
        Task<UserCredential> GetChatCredentials(long chatId, AuthorizationScope scope, CancellationToken cancellationToken);
        IEnumerable<string> GetYoutubeScope(AuthorizationScope scope);
    }

    public enum AuthorizationScope
    {
        Public,
        Readonly,
        Fullaccess,
    }

    public class YouTubeAuthorizationService : IYouTubeAuthorizationService
    {
        private readonly ITableStorage<ChatAuthorizations> tableStorage;
        private readonly IDataStore dataStore;
        private readonly ClientSecrets clientSecrets;
        private readonly IAppConfig appConfig;

        public YouTubeAuthorizationService(IDataStore dataStore, ClientSecrets clientSecrets, IAppConfig appConfig, ITableStorage<ChatAuthorizations> tableStorage)
        {
            this.dataStore = dataStore;
            this.clientSecrets = clientSecrets;
            this.appConfig = appConfig;
            this.tableStorage = tableStorage;
        }

        public async Task CreateChatCredentials(long chatId, TokenResponse token, AuthorizationScope scope, CancellationToken cancellationToken)
        {
            var item = new ChatAuthorizations
            {
                ChatId = chatId,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                ExpiresInSeconds = token.ExpiresInSeconds,
                IssuedUtc = DateTime.UtcNow,// token.IssuedUtc,
                TokenType = token.TokenType,
                Scope = token.Scope
            };
            await this.tableStorage.InsertOrMergeAsync(item);
        }

        public async Task<UserCredential> GetChatCredentials(long chatId, AuthorizationScope scope, CancellationToken cancellationToken)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = this.clientSecrets,
                Scopes = GetYoutubeScope(scope)
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = this.clientSecrets,
                DataStore = dataStore
            });

            var redirect = $"{this.appConfig.Host}/api/oauth2callback";

            var authorization = new AuthorizationCodeWebApp(flow, redirect, chatId.ToString());

            var token = await authorization.AuthorizeAsync(chatId.ToString(), cancellationToken);
            return token.Credential;
        }

        public IEnumerable<string> GetYoutubeScope(AuthorizationScope scope)
        {
            switch (scope)
            {
                case AuthorizationScope.Readonly:
                    return new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeReadonly };
                case AuthorizationScope.Fullaccess:
                    return new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeForceSsl };
                case AuthorizationScope.Public:
                default:
                    return new string[] { };
            }
        }
    }
}
