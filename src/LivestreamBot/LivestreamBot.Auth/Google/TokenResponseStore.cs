
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;

using LivestreamBot.Persistance;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamBot.Auth.Google
{
    public class TokenResponseStore : IDataStore
    {
        private readonly ITableStorage<ChatAuthorizations> chatAuthorizations;

        public TokenResponseStore(ITableStorage<ChatAuthorizations> chatAuthorizations)
        {
            this.chatAuthorizations = chatAuthorizations;
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(string key)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>(string key)
        {
            var chatId = long.Parse(key);
            var authorization = chatAuthorizations.Get().Where(auth => auth.ChatId == chatId).First();
            if (authorization == null)
            {
                return null;
            }

            var response = new TokenResponse
            {
                AccessToken = authorization.AccessToken,
                RefreshToken = authorization.RefreshToken,
                ExpiresInSeconds = authorization.ExpiresInSeconds,
                IssuedUtc = authorization.IssuedUtc,
                Scope = authorization.Scope,
                TokenType = authorization.TokenType
            };

            T result = (T)Convert.ChangeType(response, typeof(T));
            return Task.FromResult(result);
        }

        public Task StoreAsync<T>(string key, T value)
        {
            return Task.CompletedTask;
        }
    }
}
