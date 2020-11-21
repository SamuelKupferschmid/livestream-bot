using LivestreamBot.Bot.Subscriptions;
using LivestreamBot.Livestream;
using LivestreamBot.Persistance;

using MediatR;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LivestreamBot.Handlers.Livestream
{
    public class LivestreamMissingNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>
    {
        private readonly ITelegramBotSubscriptionService telegramBotSubscriptions;
        private readonly ITableStorage<LivestreamNotification> notificationTable;
        private readonly ITelegramBotClient botClient;

        public LivestreamMissingNotificationHandler(ITelegramBotClient botClient, ITelegramBotSubscriptionService telegramBotSubscriptions, ITableStorage<LivestreamNotification> notificationTable)
        {
            this.botClient = botClient;
            this.telegramBotSubscriptions = telegramBotSubscriptions;
            this.notificationTable = notificationTable;
        }

        public async Task Handle(LiveStreamNotificationInfo info, CancellationToken cancellationToken)
        {
            if(!IsMissing(info))
            {
                return;
            }

            var chats = await telegramBotSubscriptions.GetSubcribers(NotificationNames.LivestreamMissing, cancellationToken);

            var message = "Upps, irgendwie schein noch kein Livestream sichtbar zu sein. Könnt ihr da Mal schauen?";

            foreach (var chat in chats)
            {
                await botClient.SendTextMessageAsync(new ChatId(chat), message, cancellationToken: cancellationToken);
            }

            await notificationTable.InsertOrMergeAsync(new LivestreamNotification
            {
                Name = NotificationNames.LivestreamMissing,
                DateTime = DateTime.UtcNow,
            });
        }

        private bool IsMissing(LiveStreamNotificationInfo info)
        {
            var expectEvent = info.OngoingEvent || info.TimeUntilEvent < TimeSpan.FromMinutes(15);
            var foundLivestream = info.SearchResults.Any(s => s.Snippet.LiveBroadcastContent != "none");
            var hasNotified = info.ExistingNotifications.Any(not => not.Name == NotificationNames.LivestreamMissing);

            return expectEvent && !foundLivestream && !hasNotified;
        }
    }
}
