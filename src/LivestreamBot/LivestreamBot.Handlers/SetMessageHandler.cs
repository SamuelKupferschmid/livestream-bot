using LivesteamBot.Bot;

using LivestreamBot.Core;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;

namespace LivestreamBot.Functions.Handlers
{
    public class SetMessageEvent
    {
        public string Message { get; set; }
    }

    public class SetMessageHandler : IHandler<SetMessageEvent>
    {
        private readonly ITelegramBotClient botClient;
        private readonly ITelegramBotInfo botInfo;

        public SetMessageHandler(ITelegramBotClient botClient, ITelegramBotInfo botInfo)
        {
            this.botClient = botClient;
            this.botInfo = botInfo;
        }

        public async Task Handle(SetMessageEvent @event, CancellationToken cancellationToken)
        {
            await this.botClient.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(this.botInfo.OwnerChatId), @event.Message, cancellationToken: cancellationToken);
        }
    }
}
