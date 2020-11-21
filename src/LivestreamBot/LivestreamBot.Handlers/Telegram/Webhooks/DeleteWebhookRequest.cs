using MediatR;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;

namespace LivestreamBot.Handlers.Telegram.Webhooks
{
    public class DeleteWebhookRequest : IRequest { }

    public class DeleteWebhookRequestHandler : IRequestHandler<DeleteWebhookRequest>
    {
        private readonly ITelegramBotClient client;

        public DeleteWebhookRequestHandler(ITelegramBotClient client)
        {
            this.client = client;
        }

        public async Task<Unit> Handle(DeleteWebhookRequest request, CancellationToken cancellationToken)
        {
            await this.client.DeleteWebhookAsync(cancellationToken);
            return Unit.Value;
        }
    }
}