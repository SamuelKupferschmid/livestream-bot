using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram.Commands
{
    public abstract class BaseBotCommandHandler : INotificationHandler<BotUpdate>
    {
        protected abstract string Command { get; }

        public async Task Handle(BotUpdate update, CancellationToken cancellationToken)
        {
            var message = update.Update.Message;
            var matches = message != null && message.Type == MessageType.Text && message.EntityValues != null && message.EntityValues.Any(e => e == "/" + Command || e.StartsWith($"/{Command}@"));

            if(matches)
            {
                await this.Handle(message, cancellationToken);
            }
        }

        public abstract Task Handle(Message message, CancellationToken cancellationToken);
    }


}
