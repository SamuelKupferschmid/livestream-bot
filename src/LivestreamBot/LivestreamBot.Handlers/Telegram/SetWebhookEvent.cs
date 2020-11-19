using LivesteamBot.Bot;

using LivestreamBot.Core;

using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LivestreamBot.Handlers.Telegram
{
    public class SetWebhookEvent { }

    public class SetWebhookEventHandler : IHandler<SetWebhookEvent>
    {
        private readonly ITelegramBotClient botClient;
        private readonly ITelegramBotInfo botInfo;

        public SetWebhookEventHandler(ITelegramBotClient botClient, ITelegramBotInfo botInfo)
        {
            this.botClient = botClient;
            this.botInfo = botInfo;
        }

        public async Task Handle(SetWebhookEvent @event, CancellationToken cancellationToken)
        {
            var url = $"{Environment.GetEnvironmentVariable("BaseUrl")}/api/telegramwebhook/{Environment.GetEnvironmentVariable("TelegramToken")}";
            await botClient.SetWebhookAsync(url, cancellationToken: cancellationToken);
            await botClient.SendTextMessageAsync(new ChatId(botInfo.OwnerChatId), "I just set my Webhook", cancellationToken: cancellationToken);
        }
    }
}
