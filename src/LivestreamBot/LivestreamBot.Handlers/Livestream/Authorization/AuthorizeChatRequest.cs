using LivestreamBot.Auth.Google;
using LivestreamBot.Core.Environment;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Handlers.Livestream.Authorization
{
    public class AuthorizeChatRequest : IRequest<IActionResult>
    {
        public string ChatId { get; set; }
    }

    public class AuthorizeChatRequestHandler : IRequestHandler<AuthorizeChatRequest, IActionResult>
    {
        private readonly IYouTubeAuthorizationService authorizationService;
        private readonly IAppConfig appConfig;

        public AuthorizeChatRequestHandler(IYouTubeAuthorizationService authorizationService, IAppConfig appConfig)
        {
            this.authorizationService = authorizationService;
            this.appConfig = appConfig;
        }

        public async Task<IActionResult> Handle(AuthorizeChatRequest request, CancellationToken cancellationToken)
        {
            var query = new Dictionary<string, string>
            {
                {"client_id", this.appConfig.GoogleApiClientId },
                {"redirect_uri", this.appConfig.Host + "/api/oauth2callback" },
                {"scope", string.Join(" ", this.authorizationService.GetYoutubeScope(AuthorizationScope.Fullaccess)) },
                {"response_type", "code" },
                {"access_type", "offline" },
                {"state", request.ChatId }
            };

            var auth_url = QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", query);

            return await Task.FromResult(new RedirectResult(auth_url, false));
        }
    }

}
