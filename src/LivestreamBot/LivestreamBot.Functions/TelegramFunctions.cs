using System;
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
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            await FunctionsContainer.Handle(new SetMessageEvent { Message = name }, cancellationToken);

            return new OkObjectResult(responseMessage);
        }

        [FunctionName(nameof(TelegramSetWebhookAsync))]
        [NoAutomaticTrigger]
        public async Task TelegramSetWebhookAsync(string input, CancellationToken cancellationToken)
        {
            await FunctionsContainer.Handle<SetWebhookEvent>(cancellationToken);
        }

        [FunctionName(nameof(TelegramDeleteWebHookAsync))]
        [NoAutomaticTrigger]
        public async Task TelegramDeleteWebHookAsync(string input, CancellationToken cancellationToken)
        {
            await FunctionsContainer.Handle<DeleteWebhookEvent>(cancellationToken);
        }
    }
}
