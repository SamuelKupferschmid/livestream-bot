using System.Threading;
using System.Threading.Tasks;

using LivestreamBot.Functions;
using LivestreamBot.Handlers.Livestream.Authorization;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ChurchLiveStreamBot
{
    public class AuthenticationFunctions : FuncionsBase
    {
        public AuthenticationFunctions(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        [FunctionName(nameof(Authorize))]
        public async Task<IActionResult> Authorize([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "authorize/{chatId}")] HttpRequest req,
                                               string chatId,
                                               ILogger log,
                                               CancellationToken cancellationToken)
        {
            return await Send(new AuthorizeChatRequest
            {
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
            return await Send(new AuthorizationCallbackRequest
            {
                ChatId = chatId,
                Code = code,
                Error = error,
            }, cancellationToken);
        }
    }
}
