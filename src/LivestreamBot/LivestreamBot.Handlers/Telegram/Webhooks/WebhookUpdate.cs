
using LivestreamBot.Core.Environment;

using MediatR;

using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace LivestreamBot.Handlers.Telegram.Webhooks
{
    public class WebhookUpdate : IRequest
    {
        public Update Payload { get; set; }
        public string Token { get; set; }
    }

    public class WebhookUpdateHandler : IRequestHandler<WebhookUpdate>
    {
        private readonly IAppConfig appConfig;
        private readonly IMediator mediator;

        public WebhookUpdateHandler(IMediator mediator, IAppConfig appConfig)
        {
            this.mediator = mediator;
            this.appConfig = appConfig;
        }

        public async Task<Unit> Handle(WebhookUpdate request, CancellationToken cancellationToken)
        {
            if (request.Token != appConfig.TelegramToken)
            {
                // Throw proprer unauthorized Exception
                throw new Exception();
            }

            var update = new BotUpdate(request.Payload);
            await mediator.Publish(update, cancellationToken);

            return Unit.Value;
        }
    }

}