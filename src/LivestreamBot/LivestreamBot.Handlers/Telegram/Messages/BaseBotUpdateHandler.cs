
using MediatR;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot.Types;

namespace LivestreamBot.Handlers.Telegram
{

    public class BotUpdate : INotification
    {
        public BotUpdate(Update update)
        {
            this.Update = update;
        }

        public string Token { get; }

        public Update Update { get; }
    }

    public abstract class BaseBotUpdateHandler : INotificationHandler<BotUpdate>
    {
        protected abstract bool Predicate(BotUpdate update);
        public abstract Task HandleUpdate(BotUpdate update, CancellationToken cancellationToken);

        public async Task Handle(BotUpdate update, CancellationToken cancellationToken)
        {
            if(Predicate(update))
            {
                await this.HandleUpdate(update, cancellationToken);
            }
        }
    }
}