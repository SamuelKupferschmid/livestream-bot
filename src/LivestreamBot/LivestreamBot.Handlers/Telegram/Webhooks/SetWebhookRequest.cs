using LivestreamBot.Core.Environment;

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
        private readonly IAppConfig appConfig;
        public SetWebhookRequestHandler(ITelegramBotClient client, IAppConfig appConfig)
        {
            this.client = client;
            this.appConfig = appConfig;
        }

        public async Task<Unit> Handle(SetWebhookRequest request, CancellationToken cancellationToken)
        {
            var url = $"{appConfig.Host}/api/telegram-webhook/{appConfig.TelegramToken}";
            await client.SetWebhookAsync(url, cancellationToken: cancellationToken);
            return Unit.Value;
        }
    }
}