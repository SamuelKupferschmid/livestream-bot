
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util;
using Google.Apis.YouTube.v3.Data;

using LivestreamBot.Auth.Google;
using LivestreamBot.Core.Environment;
using LivestreamBot.Livestream;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace LivestreamBot.Handlers.Livestream.Authorization
{
    public class AuthorizationCallbackRequest : IRequest<IActionResult>
    {
        public string Code { get; set; }
        public string Error { get; set; }
        public long ChatId { get; set; }
    }



    public class AuthorizationCallbackRequestHandler : IRequestHandler<AuthorizationCallbackRequest, IActionResult>
    {
        private readonly IAppConfig appConfig;
        private readonly IYouTubeAuthorizationService authorizationService;
        private readonly IYoutubeServiceProvider youtubeServiceProvider;
        private readonly ITelegramBotClient botClient;
        private readonly ILogger<AuthorizationCallbackRequestHandler> logger;

        public AuthorizationCallbackRequestHandler(IAppConfig appConfig,
                                                   IYouTubeAuthorizationService authorizationService,
                                                   IYoutubeServiceProvider youtubeServiceProvider,
                                                   ITelegramBotClient botClient,
                                                   ILogger<AuthorizationCallbackRequestHandler> logger)
        {
            this.appConfig = appConfig;
            this.authorizationService = authorizationService;
            this.youtubeServiceProvider = youtubeServiceProvider;
            this.botClient = botClient;
            this.logger = logger;
        }

        public async Task<IActionResult> Handle(AuthorizationCallbackRequest request, CancellationToken cancellationToken)
        {
            if(request.Error != null)
            {
                throw new InvalidOperationException($"Goolge API Authorization led to an Error: {request.Error}");
            }

            if (request.Code == null) throw new ArgumentNullException(nameof(request.Code));
            if (request.ChatId == 0) throw new ArgumentOutOfRangeException(nameof(request.ChatId));

            using var http = new HttpClient();
            var redirect = $"{this.appConfig.Host}/api/oauth2callback";

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", request.Code),
                new KeyValuePair<string, string>("client_id", this.appConfig.GoogleApiClientId),
                new KeyValuePair<string, string>("client_secret", this.appConfig.GoogleApiClientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirect),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            });

            logger.LogInformation("retrieving OAuth2 Tokens from Google API for ChatId {ChatId}", request.ChatId);

            var response = await http.PostAsync($"https://oauth2.googleapis.com/token", formContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError("retrieving OAuth Tokens failed with HTTP Code {code}, with Reason {reason}", response.StatusCode,response.ReasonPhrase);
            }


            var token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

            await this.authorizationService.CreateChatCredentials(request.ChatId, token, AuthorizationScope.Fullaccess, cancellationToken);

            var channels = await GetChannels(request.ChatId, cancellationToken);

            var buttons = channels.Select(channel => new InlineKeyboardButton {
                Text = channel.Snippet.Title,
                CallbackData = $"setchannel: {channel.Id}"
            }); 
            
            var keyboard = new InlineKeyboardMarkup(buttons.Select(button => new InlineKeyboardButton[] { button }));

            await botClient.SendTextMessageAsync(request.ChatId,
                                           "Ich scheine Zugriff auf dein YouTube Konto zu haben. Keine Angst, ich werde kein Unfug treiben.\nBitte wähle den gewünschten Channel aus, um das Setup abzuschliessen:",
                                           replyMarkup: keyboard,
                                           cancellationToken: cancellationToken);

            return new OkObjectResult("Authorisierung erfolgreich. Du kannst nun zurück zu Telegram wechseln.");
        }

        private async Task<IList<Channel>> GetChannels(long chatId, CancellationToken cancellationToken)
        {
            var youTube = await this.youtubeServiceProvider.GetService(chatId, cancellationToken);

            var request = youTube.Channels.List(new Repeatable<string>(new[] { "snippet", "contentDetails" }));

            request.Mine = true;

            var response = await request.ExecuteAsync(cancellationToken);
            return response.Items;
        }
    }

}
