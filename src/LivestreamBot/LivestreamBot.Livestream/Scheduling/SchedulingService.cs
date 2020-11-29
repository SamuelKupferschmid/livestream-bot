using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using LivestreamBot.Auth;
using LivestreamBot.Core;
using LivestreamBot.Livestream.Events;
using LivestreamBot.Persistance;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Livestream.Scheduling
{
    public interface ISchedulingService
    {
        IEnumerable<(long chatId, string channelId)> GetEventSubscriptions(LivestreamEvent @event);
        Task<LiveBroadcast> ScheduleEvent(LivestreamEvent @event, string channelId, long chatId, DateTime startTime, CancellationToken cancellationToken);
    }

    public class SchedulingService : ISchedulingService
    {
        private readonly IYoutubeServiceProvider serviceProvider;
        private readonly ITableStorage<ChatAuthorization> chatAuthorizations;

        public SchedulingService(IYoutubeServiceProvider serviceProvider, ITableStorage<ChatAuthorization> chatAuthorizations)
        {
            this.serviceProvider = serviceProvider;
            this.chatAuthorizations = chatAuthorizations;
        }

        public IEnumerable<(long chatId, string channelId)> GetEventSubscriptions(LivestreamEvent @event)
        {
            var subscriptions = this.chatAuthorizations.Get().ToList();

            return subscriptions.Select(sub => (sub.ChatId, sub.ChannelId));
        }

        public async Task<LiveBroadcast> ScheduleEvent(LivestreamEvent @event, string channelId, long chatId, DateTime startTime, CancellationToken cancellationToken)
        {
            var service = await serviceProvider.GetService(chatId, cancellationToken);
            var stream = service.LiveBroadcasts.Insert(new LiveBroadcast
            {
                Snippet = new LiveBroadcastSnippet
                {
                    Title = $"ICF Brugg | {@event.Identifier} Celebration",
                    ScheduledStartTime = startTime.ToRfc3339String(),
                    ChannelId = channelId,
                }, Status = new LiveBroadcastStatus
                {
                    PrivacyStatus = "public"
                }
            }, new Google.Apis.Util.Repeatable<string>(new[] { "snippet", "status" }));

            return await stream.ExecuteAsync(cancellationToken);
        }
    }
}
