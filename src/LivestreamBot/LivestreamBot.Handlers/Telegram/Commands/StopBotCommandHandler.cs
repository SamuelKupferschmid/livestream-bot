using LivestreamBot.Bot.Subscriptions;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram.Commands
{
    public class StopBotCommandHandler : BaseBotCommandHandler
    {
        private readonly ITelegramBotClient client;
        private readonly ITelegramBotSubscriptionService botSubscriptionService;

        protected override string Command => "stop";

        public StopBotCommandHandler(ITelegramBotClient client, ITelegramBotSubscriptionService botSubscriptionService)
        {
            this.client = client;
            this.botSubscriptionService = botSubscriptionService;
        }

        public override async Task Handle(Message message, CancellationToken cancellationToken)
        {
            await this.botSubscriptionService.UnsubscribeAll(message.Chat.Id, cancellationToken);

            var plural = message.Chat.Type == ChatType.Private;
            await client.SendTextMessageAsync(message.Chat.Id, $"*Hallo {message.From.FirstName}!! 😊* Ich sage ab sofort nichts mehr.. 😳", ParseMode.Markdown, cancellationToken: cancellationToken);

            await Task.Delay(3000);

            await client.SendTextMessageAsync(message.Chat.Id, $"Ok, tschüss!! ", ParseMode.Markdown, cancellationToken: cancellationToken);
            await Task.Delay(10000);
            await client.SendTextMessageAsync(message.Chat.Id, $"😘", ParseMode.Markdown, cancellationToken: cancellationToken);
        }

    }
}