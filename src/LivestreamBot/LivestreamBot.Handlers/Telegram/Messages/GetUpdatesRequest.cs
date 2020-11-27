
using MediatR;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;

namespace LivestreamBot.Handlers.Telegram.Messages
{
    public class GetUpdatesRequest : IRequest
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
            int rounds = 2;
            int offset = 0;
            while (rounds-- > 0)
            {
                var updates = await botClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken);

                foreach (var update in updates)
                {
                    offset = update.Id + 1;
                    await mediator.Publish(new BotUpdate(update), cancellationToken);
                }
                await Task.Delay(1000);
            }

            return Unit.Value;
        }
    }
}