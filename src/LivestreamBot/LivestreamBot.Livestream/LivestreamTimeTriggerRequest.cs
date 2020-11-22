using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using LivestreamBot.Core;
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
        private readonly YouTubeService service;
        private readonly string channelId;

        public LivestreamTimeTriggerRequestHandler(ITableStorage<LivestreamNotification> tableStorage, IMediator mediator)
        {
            this.tableStorage = tableStorage;
            this.service = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = Environment.GetEnvironmentVariable("YoutubeApiKey")
            });

            channelId = Environment.GetEnvironmentVariable("YoutubeChannelId");
            this.mediator = mediator;
        }

        public async Task<Unit> Handle(LivestreamTimeTriggerRequest request, CancellationToken cancellationToken)
        {
            var dateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Europe/Zurich"));

            var timeOfDay = dateTime.TimeOfDay;


            var start1 = new TimeSpan(10, 45, 00);
            var end1 = new TimeSpan(12, 00, 00);
            var start2 = new TimeSpan(19, 15, 00);
            var end2 = new TimeSpan(20, 30, 00);

            var info = new LiveStreamNotificationInfo
            {
                OngoingEvent = timeOfDay > start1 && timeOfDay < end1 || timeOfDay > start2 && timeOfDay < end2,
            };

            if (timeOfDay < start1)
            {
                info.TimeUntilEvent = start1 - timeOfDay;
            }
            else if (timeOfDay < start2)
            {
                info.TimeUntilEvent = start2 - timeOfDay;
            }

            if (timeOfDay > end2)
            {
                info.TimeSinceEvent = timeOfDay - end2;
            }
            else if (timeOfDay > end1)
            {
                info.TimeSinceEvent = timeOfDay - end1;
            }

            var tolerance = TimeSpan.FromHours(2);


            if (dateTime.DayOfWeek != DayOfWeek.Sunday || !(info.OngoingEvent || info.TimeSinceEvent < tolerance || info.TimeUntilEvent < tolerance))
            {
                return Unit.Value;
            }

            var list = service.Search.List(new Google.Apis.Util.Repeatable<string>(new[] { "snippet" }));

            list.ChannelId = channelId;
            list.Type = new Google.Apis.Util.Repeatable<string>(new[] { "video" });
            list.PublishedAfter = DateTime.UtcNow.AddHours(-4).ToRfc3339String();

            info.SearchResults = (await list.ExecuteAsync()).Items;
            info.ExistingNotifications = tableStorage.Get().Where(n => n.DateTime > DateTime.UtcNow.AddHours(-4)).ToList();

            await mediator.Publish(info, cancellationToken);

            return Unit.Value;
        }
    }

    public class LiveStreamNotificationInfo: INotification
    {
        public IList<SearchResult> SearchResults { get; set; }
        public IList<LivestreamNotification> ExistingNotifications { get; set; }
        public TimeSpan? TimeUntilEvent { get; set; }
        public TimeSpan? TimeSinceEvent { get; set; }
        public bool OngoingEvent { get; set; }
    }
}
