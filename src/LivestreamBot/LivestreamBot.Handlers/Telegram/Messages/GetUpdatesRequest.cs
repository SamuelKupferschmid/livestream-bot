
using MediatR;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;

namespace LivestreamBot.Handlers.Telegram
{
    public class GetUpdatesRequest: IRequest
    {

    }

    public class GetUpdatesRequestHandler : IRequestHandler<GetUpdatesRequest>
    {
        private readonly ITelegramBotClient botClient;
        private readonly IMediator mediator;

        public GetUpdatesRequestHandler(ITelegramBotClient botClient, IMediator mediator)
        {
            this.botClient = botClient;
            this.mediator = mediator;
        }

        public async Task<Unit> Handle(GetUpdatesRequest request, CancellationToken cancellationToken)
        {
            var updates = await this.botClient.GetUpdatesAsync(cancellationToken: cancellationToken);

            foreach(var update in updates)
            {
                await mediator.Publish(new BotUpdate(update), cancellationToken);
            }

            return Unit.Value;
        }
    }
}