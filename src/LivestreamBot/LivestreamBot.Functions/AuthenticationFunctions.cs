using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Functions;
using LivestreamBot.Handlers.Livestream.Authorization;
using LivestreamBot.Livestream.Notifications;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChurchLiveStreamBot
{
    public class AuthenticationFunctions
    {
        [FunctionName(nameof(Authorize))]
        public async Task<IActionResult> Authorize([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authorize/{chatId}")] HttpRequest req,
                                               string chatId,
                                               ILogger log,
                                               CancellationToken cancellationToken)
        {
            return await FunctionsMediator.Send(new AuthorizeChatRequest { 
                ChatId = chatId 
            }, cancellationToken);
        }

        [FunctionName(nameof(Oauth2callback))]
        public async Task<IActionResult> Oauth2callback([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "oauth2callback")] HttpRequest req,
                                                             ILogger log,
                                                             CancellationToken cancellationToken)
        {
            var code = req.Query["code"];
            var chatId = long.Parse(req.Query["state"]);
            var error = req.Query["error"];
            return await FunctionsMediator.Send(new AuthorizationCallbackRequest { 
                ChatId = chatId, 
                Code = code ,
                Error = error,
            }, cancellationToken);
        }
    }
}
