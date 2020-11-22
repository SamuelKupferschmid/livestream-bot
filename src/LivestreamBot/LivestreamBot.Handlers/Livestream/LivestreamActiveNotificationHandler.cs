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
    public class LivestreamActiveNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>
    {
        private readonly ITelegramBotSubscriptionService telegramBotSubscriptions;
        private readonly ITableStorage<LivestreamNotification> notificationTable;
        private readonly ITelegramBotClient botClient;

        public LivestreamActiveNotificationHandler(ITelegramBotClient botClient, ITelegramBotSubscriptionService telegramBotSubscriptions, ITableStorage<LivestreamNotification> notificationTable)
        {
            this.botClient = botClient;
            this.telegramBotSubscriptions = telegramBotSubscriptions;
            this.notificationTable = notificationTable;
        }

        public async Task Handle(LiveStreamNotificationInfo info, CancellationToken cancellationToken)
        {
            if (!IsActive(info))
            {
                return;
            }

            foreach (var result in info.SearchResults)
            {
                if (result.Snippet.LiveBroadcastContent != "live" && info.ExistingNotifications.Any(not => not.VideoId == result.Id.VideoId && not.Name == NotificationNames.LivestreamActive))
                {
                    continue;
                }

                var videoLink = $@"Wir sind Live!!
https://www.youtube.com/watch?v={result.Id.VideoId}";

                foreach (var chat in await telegramBotSubscriptions.GetSubcribers(NotificationNames.LivestreamActive, cancellationToken))
                {
                    await this.botClient.SendTextMessageAsync(new ChatId(chat), videoLink, cancellationToken: cancellationToken);
                }

                await notificationTable.InsertOrMergeAsync(new LivestreamNotification
                {
                    Name = NotificationNames.LivestreamActive,
                    DateTime = DateTime.UtcNow,
                    VideoId = result.Id.VideoId,
                });
            }
        }

        private bool IsActive(LiveStreamNotificationInfo info)
        {
            var foundLivestream = info.SearchResults.Any(s => s.Snippet.LiveBroadcastContent == "live");
            var hasNotified = info.ExistingNotifications.Any(not => not.Name == NotificationNames.LivestreamActive);

            return foundLivestream && !hasNotified;
        }
    }
}
