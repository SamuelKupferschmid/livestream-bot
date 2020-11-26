using Google.Apis.YouTube.v3.Data;

using LivestreamBot.Livestream.Events;

using MediatR;

using System;
using System.Collections.Generic;

namespace LivestreamBot.Livestream.Notifications
{
    public class LiveStreamNotificationInfo : INotification
    {
        public IList<SearchResult> SearchResults { get; set; }
        public IList<LivestreamNotification> ExistingNotifications { get; set; }
        public LivestreamEvent Event { get; set; }
        public bool IsOngoing { get; set; }
        public TimeSpan TimeUntilNext { get; set; }
    }
}
