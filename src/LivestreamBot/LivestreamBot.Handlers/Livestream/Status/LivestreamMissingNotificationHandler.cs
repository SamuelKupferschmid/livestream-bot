using LivestreamBot.Bot.Subscriptions;
using LivestreamBot.Persistance;
using LivestreamBot.Livestream.Events;
using MediatR;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;
using LivestreamBot.Livestream.Notifications;

namespace LivestreamBot.Handlers.Livestream.Status
{
    public class LivestreamMissingNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>, ILivestreamTimeTriggeredEventNotificationHandler
    {
        private readonly ITelegramBotSubscriptionService telegramBotSubscriptions;
        private readonly ITableStorage<LivestreamNotification> notificationTable;
        private readonly TimeZoneInfo timezoneInfo;
        private readonly ITelegramBotClient botClient;

        public LivestreamMissingNotificationHandler(ITelegramBotClient botClient, ITelegramBotSubscriptionService telegramBotSubscriptions, ITableStorage<LivestreamNotification> notificationTable, TimeZoneInfo timezoneInfo)
        {
            this.botClient = botClient;
            this.telegramBotSubscriptions = telegramBotSubscriptions;
            this.notificationTable = notificationTable;
            this.timezoneInfo = timezoneInfo;
        }

        public TimeSpan NotifyBeforeLivestream => TimeSpan.FromMinutes(25);

        public async Task Handle(LiveStreamNotificationInfo info, CancellationToken cancellationToken)
        {
            if (!IsMissing(info))
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
            var expectEvent = info.IsOngoing || info.TimeUntilNext < NotifyBeforeLivestream;
            var foundLivestream = info.SearchResults.Any(s => s.Snippet.LiveBroadcastContent != "none");
            var hasNotified = info.ExistingNotifications.Any(not => not.Name == NotificationNames.LivestreamMissing);

            return expectEvent && !foundLivestream && !hasNotified;
        }
    }
}
