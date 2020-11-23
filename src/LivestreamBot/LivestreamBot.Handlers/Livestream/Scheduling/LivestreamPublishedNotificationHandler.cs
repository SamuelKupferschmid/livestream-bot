using LivestreamBot.Bot.Subscriptions;
using LivestreamBot.Persistance;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Linq;
using LivestreamBot.Livestream.Notifications;

namespace LivestreamBot.Handlers.Livestream.Scheduling
{
    public class LivestreamPublishedNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>
    {
        private readonly ITelegramBotSubscriptionService telegramBotSubscriptions;
        private readonly ITableStorage<LivestreamNotification> notificationTable;
        private readonly ITelegramBotClient botClient;

        public LivestreamPublishedNotificationHandler(ITelegramBotClient botClient, ITelegramBotSubscriptionService telegramBotSubscriptions, ITableStorage<LivestreamNotification> notificationTable)
        {
            this.botClient = botClient;
            this.telegramBotSubscriptions = telegramBotSubscriptions;
            this.notificationTable = notificationTable;
        }

        public async Task Handle(LiveStreamNotificationInfo info, CancellationToken cancellationToken)
        {
            if (!HasNewLivestreams(info))
            {
                return;
            }

            foreach (var result in info.SearchResults)
            {
                if (result.Snippet.LiveBroadcastContent == "none" && info.ExistingNotifications.Any(not => not.VideoId == result.Id.VideoId && not.Name == NotificationNames.LivestreamNew))
                {
                    continue;
                }

                var videoLink = $"Hallo, ein neuer Livestream ist verfügbar: https://www.youtube.com/watch?v={result.Id.VideoId} .";

                foreach (var chat in await telegramBotSubscriptions.GetSubcribers(NotificationNames.LivestreamNew, cancellationToken))
                {
                    await botClient.SendTextMessageAsync(new ChatId(chat), videoLink, cancellationToken: cancellationToken);
                }

                await notificationTable.InsertOrMergeAsync(new LivestreamNotification
                {
                    Name = NotificationNames.LivestreamNew,
                    DateTime = DateTime.UtcNow,
                    VideoId = result.Id.VideoId,
                });
            }
        }

        private bool HasNewLivestreams(LiveStreamNotificationInfo info)
        {

            var hasNotified = info.ExistingNotifications.Any(not => not.Name == NotificationNames.LivestreamNew);
            var hasLivestraem = info.SearchResults.Any(result => result.Snippet.LiveBroadcastContent != "none");
            return !hasNotified && hasLivestraem;
        }
    }
}
