
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
        private readonly string token;
        private readonly IMediator mediator;

        public WebhookUpdateHandler(IMediator mediator)
        {
            token = Environment.GetEnvironmentVariable("TelegramToken");
            this.mediator = mediator;
        }

        public async Task<Unit> Handle(WebhookUpdate request, CancellationToken cancellationToken)
        {
            if (request.Token != token)
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