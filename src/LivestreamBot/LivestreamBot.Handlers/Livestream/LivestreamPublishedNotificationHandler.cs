using LivestreamBot.Bot.Subscriptions;
using LivestreamBot.Livestream;
using LivestreamBot.Persistance;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Linq;

namespace LivestreamBot.Handlers.Livestream
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
                if (result.Snippet.LiveBroadcastContent == "none" && info.ExistingNotifications.Any(not => not.VideoId == result.Id.VideoId && not.Name == NotificationNames.NewLivestream))
                {
                    continue;
                }

                var videoLink = $"Hallo, ein neuer Livestream ist verfügbar: https://www.youtube.com/watch?v={result.Id.VideoId} .";

                foreach (var chat in await telegramBotSubscriptions.GetSubcribers(NotificationNames.NewLivestream, cancellationToken))
                {
                    await this.botClient.SendTextMessageAsync(new ChatId(chat), videoLink, cancellationToken: cancellationToken);
                }

                await notificationTable.InsertOrMergeAsync(new LivestreamNotification
                {
                    Name = NotificationNames.NewLivestream,
                    DateTime = DateTime.UtcNow,
                    VideoId = result.Id.VideoId,
                });
            }
        }

        private bool HasNewLivestreams(LiveStreamNotificationInfo info)
        {

            var hasNotified = info.ExistingNotifications.Any(not => not.Name == NotificationNames.NewLivestream);
            var hasLivestraem = info.SearchResults.Any(result => result.Snippet.LiveBroadcastContent != "none");
            return !hasNotified && hasLivestraem;
        }
    }
}
