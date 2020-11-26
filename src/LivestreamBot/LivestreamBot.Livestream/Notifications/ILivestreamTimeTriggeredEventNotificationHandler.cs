
using MediatR;

using System;

namespace LivestreamBot.Livestream.Notifications
{
    public interface ILivestreamTimeTriggeredEventNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>
    {
        TimeSpan NotifyBeforeLivestream { get; }
    }
}
