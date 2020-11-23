using LivestreamBot.Bot.Subscriptions;
using LivestreamBot.Livestream.Notifications;
using LivestreamBot.Persistance;

using MediatR;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace LivestreamBot.Handlers.Livestream.Status
{
    public class LivestreamNotActiveNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>, ILivestreamTimeTriggeredEventNotificationHandler
    {
        private readonly ITelegramBotSubscriptionService telegramBotSubscriptions;
        private readonly ITableStorage<LivestreamNotification> notificationTable;
        private readonly ITelegramBotClient botClient;

        public LivestreamNotActiveNotificationHandler(ITelegramBotClient botClient, ITelegramBotSubscriptionService telegramBotSubscriptions, ITableStorage<LivestreamNotification> notificationTable)
        {
            this.botClient = botClient;
            this.telegramBotSubscriptions = telegramBotSubscriptions;
            this.notificationTable = notificationTable;
        }

        public TimeSpan NotifyBeforeLivestream => TimeSpan.Zero;

        public async Task Handle(LiveStreamNotificationInfo info, CancellationToken cancellationToken)
        {
            if (!IsNotLive(info))
            {
                return;
            }

            var chats = await telegramBotSubscriptions.GetSubcribers(NotificationNames.LivestreamNotActive, cancellationToken);

            var message = "Hey Leute, es ist Zeit Live zu gehen aber der Livestream scheint noch nicht gestartet zu sein.";

            foreach (var chat in chats)
            {
                await botClient.SendTextMessageAsync(new ChatId(chat), message, cancellationToken: cancellationToken);
            }

            await notificationTable.InsertOrMergeAsync(new LivestreamNotification
            {
                Name = NotificationNames.LivestreamNotActive,
                DateTime = DateTime.UtcNow,
            });
        }

        private bool IsNotLive(LiveStreamNotificationInfo info)
        {

            var expectEvent = info.IsOngoing;
            var foundLivestream = info.SearchResults.Any(s => s.Snippet.LiveBroadcastContent == "active");
            var hasNotified = info.ExistingNotifications.Any(not => not.Name == NotificationNames.LivestreamNotActive
            );

            return expectEvent && !foundLivestream && !hasNotified;
        }
    }
}
