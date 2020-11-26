using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using Telegram.Bot.Types;
using LivestreamBot.Handlers.Telegram.Webhooks;
using LivestreamBot.Handlers.Telegram.Messages;

namespace LivestreamBot.Functions
{
    public class TelegramFunctions
    {
        [FunctionName(nameof(TelegramWebhook))]
        public async Task<IActionResult> TelegramWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "telegram-webhook/{token}")] HttpRequest req, string token,
            ILogger log, CancellationToken cancellationToken)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Update>(requestBody);

            await FunctionsMediator.Send(new WebhookUpdate { Payload = update, Token = token }, cancellationToken);

            return new OkResult();
        }

#if DEBUG
        [FunctionName(nameof(Trigger))]
        public async Task Trigger([TimerTrigger("*/10 * * * * *", RunOnStartup = true)] TimerInfo timer, CancellationToken cancellationToken)
        {
            await FunctionsMediator.Send(new DeleteWebhookRequest(), cancellationToken);
            await FunctionsMediator.Send(new GetUpdatesRequest(), cancellationToken);
        }
#endif

        [FunctionName(nameof(TelegramSetWebhookAsync))]
        [NoAutomaticTrigger]
        public async Task TelegramSetWebhookAsync(string input, CancellationToken cancellationToken)
        {
            await FunctionsMediator.Send(new SetWebhookRequest(), cancellationToken);
        }

        [FunctionName(nameof(TelegramDeleteWebHookAsync))]
        [NoAutomaticTrigger]
        public async Task TelegramDeleteWebHookAsync(string input, CancellationToken cancellationToken)
        {
            await FunctionsMediator.Send(new DeleteWebhookRequest(), cancellationToken);
        }
    }
}
