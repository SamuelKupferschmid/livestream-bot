using Google.Apis.Services;
using Google.Apis.YouTube.v3;

using LivestreamBot.Core;
using LivestreamBot.Livestream.Events;
using LivestreamBot.Persistance;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LivestreamBot.Livestream
{
    public class LivestreamTimeTriggerRequest : IRequest { }

    public class LivestreamTimeTriggerRequestHandler : IRequestHandler<LivestreamTimeTriggerRequest>
    {
        private readonly ITableStorage<LivestreamNotification> tableStorage;
        private readonly IMediator mediator;
        private readonly ILivestreamEventProvider eventProvider;
        private readonly IEnumerable<ILivestreamTimeTriggeredEventNotificationHandler> notificationHandlers;
        private readonly TimeZoneInfo timezoneInfo;
        private readonly YouTubeService service;
        private readonly string channelId;

        public LivestreamTimeTriggerRequestHandler(ITableStorage<LivestreamNotification> tableStorage, IMediator mediator, TimeZoneInfo timezoneInfo, ILivestreamEventProvider eventProvider, IEnumerable<ILivestreamTimeTriggeredEventNotificationHandler> notificationHandlers)
        {
            this.tableStorage = tableStorage;
            this.service = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = Environment.GetEnvironmentVariable("YoutubeApiKey")
            });

            channelId = Environment.GetEnvironmentVariable("YoutubeChannelId");
            this.mediator = mediator;
            this.timezoneInfo = timezoneInfo;
            this.eventProvider = eventProvider;
            this.notificationHandlers = notificationHandlers;
        }

        public async Task<Unit> Handle(LivestreamTimeTriggerRequest request, CancellationToken cancellationToken)
        {
            var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, this.timezoneInfo);
            var events = this.eventProvider.GetWeeklyEvents().ToList();
            var handlers = notificationHandlers.Max(handlers => handlers.NotifyBeforeLivestream);
            var timeUntilNextEvent = events.Min(ev => ev.GetLivestreamEventInfo(dateTime).untilNext);

            if (timeUntilNextEvent > handlers)
            {
                return Unit.Value;
            }

            var list = service.Search.List(new Google.Apis.Util.Repeatable<string>(new[] { "snippet" }));

            list.ChannelId = channelId;
            list.Type = new Google.Apis.Util.Repeatable<string>(new[] { "video" });
            list.PublishedAfter = DateTime.UtcNow.AddHours(-4).ToRfc3339String();

            var searchResult = (await list.ExecuteAsync()).Items;
            var existingNotifications = tableStorage.Get().Where(n => n.DateTime > DateTime.UtcNow.AddHours(-4)).ToList();

            foreach(var @event in events)
            {
                var info = new LiveStreamNotificationInfo {
                    Event = @event,
                    SearchResults = searchResult,
                    ExistingNotifications = existingNotifications,
                };
                (info.TimeUntilNext, info.IsOngoing) = @event.GetLivestreamEventInfo(dateTime);

                await mediator.Publish(info, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
