using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;

namespace LivestreamBot.Handlers.Telegram.Webhooks
{
    public class SetWebhookRequest : IRequest { }

    public class SetWebhookRequestHandler : IRequestHandler<SetWebhookRequest>
    {
        private readonly ITelegramBotClient client;
        public SetWebhookRequestHandler(ITelegramBotClient client)
        {
            this.client = client;
        }

        public async Task<Unit> Handle(SetWebhookRequest request, CancellationToken cancellationToken)
        {
            var baseurl = Environment.GetEnvironmentVariable("BaseUrl");
            var token = Environment.GetEnvironmentVariable("TelegramToken");

            var url = $"{baseurl}/api/telegram-webhook/{token}";
            await client.SetWebhookAsync(url, cancellationToken: cancellationToken);
            return Unit.Value;
        }
    }
}