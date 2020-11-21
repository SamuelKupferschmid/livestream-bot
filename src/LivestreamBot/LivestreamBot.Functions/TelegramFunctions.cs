using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using LivestreamBot.Handlers.Telegram;

namespace LivestreamBot.Functions
{
    public class TelegramFunctions
    {
        [FunctionName(nameof(TelegramWebhook))]
        public async Task<IActionResult> TelegramWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log, CancellationToken cancellationToken)
        {

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            await FunctionsContainer.Mediator.Publish(new BotUpdate(null), cancellationToken);

            return new OkObjectResult(responseMessage);
        }

        [FunctionName(nameof(Trigger))]
        public async Task Trigger([TimerTrigger("*/10 * * * * *", RunOnStartup = true)] TimerInfo timer, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            //await FunctionsContainer.Mediator.Send(new GetUpdatesRequest(), cancellationToken);
        }

        [FunctionName(nameof(TelegramSetWebhookAsync))]
        [NoAutomaticTrigger]
        public async Task TelegramSetWebhookAsync(string input, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        [FunctionName(nameof(TelegramDeleteWebHookAsync))]
        [NoAutomaticTrigger]
        public async Task TelegramDeleteWebHookAsync(string input, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
