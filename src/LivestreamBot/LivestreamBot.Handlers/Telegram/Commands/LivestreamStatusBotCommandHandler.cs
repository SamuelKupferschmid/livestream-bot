using LivestreamBot.Bot.Subscriptions;

using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LivestreamBot.Handlers.Telegram.Commands
{
    public class LivestreamStatusBotCommandHandler : BaseBotCommandHandler
    {
        private readonly ITelegramBotClient client;
        private readonly ITelegramBotSubscriptionService botSubscriptionService;

        protected override string Command => "livestreamstatus";

        public LivestreamStatusBotCommandHandler(ITelegramBotClient client, ITelegramBotSubscriptionService botSubscriptionService)
        {
            this.client = client;
            this.botSubscriptionService = botSubscriptionService;
        }

        public override async Task Handle(Message message, CancellationToken cancellationToken)
        {
            await this.botSubscriptionService.Subscribe(message.Chat.Id, NotificationNames.LivestreamMissing, cancellationToken);
            await this.botSubscriptionService.Subscribe(message.Chat.Id, NotificationNames.LivestreamNotActive, cancellationToken);
            var plural = message.Chat.Type == ChatType.Private;
            await client.SendTextMessageAsync(message.Chat.Id, $"*Hallo {message.From.FirstName}!! 😊* Ich werde ab sofort ein Auge auf unser 🎥Livesteams🎥 werfen und  {(plural ? "euch" : "dich")} informieren. Falls ich {(plural ? "euch" : "dir")} zu lästig werde, bin ich mit einem '/stop' sofort ruhig.", ParseMode.Markdown, cancellationToken: cancellationToken);

        }
    }
}