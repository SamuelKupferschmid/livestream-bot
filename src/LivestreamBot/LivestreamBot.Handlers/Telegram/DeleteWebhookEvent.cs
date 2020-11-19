using LivesteamBot.Bot;

using LivestreamBot.Core;

using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LivestreamBot.Handlers.Telegram
{
    public class DeleteWebhookEvent { }
    public class DeleteWebhookEventHandler : IHandler<DeleteWebhookEvent>
    {
        private readonly ITelegramBotClient botClient;
        private readonly ITelegramBotInfo botInfo;

        public DeleteWebhookEventHandler(ITelegramBotClient botClient, ITelegramBotInfo botInfo)
        {
            this.botClient = botClient;
            this.botInfo = botInfo;
        }

        public async Task Handle(DeleteWebhookEvent @event, CancellationToken cancellationToken)
        {
            var url = $"{Environment.GetEnvironmentVariable("BaseUrl")}/api/telegramwebhook/{Environment.GetEnvironmentVariable("TelegramToken")}";
            await botClient.DeleteWebhookAsync(cancellationToken);
            await botClient.SendTextMessageAsync(new ChatId(botInfo.OwnerChatId), "I just deleted my Webhook", cancellationToken: cancellationToken);
        }
    }
}
