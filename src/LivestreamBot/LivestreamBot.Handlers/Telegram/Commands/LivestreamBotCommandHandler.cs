using LivestreamBot.Bot.Subscriptions;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram.Commands
{
    public class LivestreamBotCommandHandler : BaseBotCommandHandler
    {
        private readonly ITelegramBotClient client;
        private readonly ITelegramBotSubscriptionService botSubscriptionService;

        protected override string Command => "livestream";

        public LivestreamBotCommandHandler(ITelegramBotClient client, ITelegramBotSubscriptionService botSubscriptionService)
        {
            this.client = client;
            this.botSubscriptionService = botSubscriptionService;
        }

        public override async Task Handle(Message message, CancellationToken cancellationToken)
        {
            await this.botSubscriptionService.Subscribe(message.Chat.Id, NotificationNames.LivestreamNew, cancellationToken);

            var text = $"*Hallo {message.From.FirstName}!! 😊* Ab sofort werde ich mich hier mitteilen, wenn eine theologische Unterweisung auf Youtube 📺 zur Verfügung steht. Falls ich zu lästig werde, bin ich mit einem '/stop' sofort ruhig.";
            await client.SendTextMessageAsync(message.Chat.Id, text, ParseMode.Markdown, cancellationToken: cancellationToken);
        }
    }
}
