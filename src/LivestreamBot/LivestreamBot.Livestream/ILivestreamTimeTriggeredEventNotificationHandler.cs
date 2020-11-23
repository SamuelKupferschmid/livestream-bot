
using MediatR;

using System;

namespace LivestreamBot.Livestream
{
    public interface ILivestreamTimeTriggeredEventNotificationHandler : INotificationHandler<LiveStreamNotificationInfo>
    {
        TimeSpan NotifyBeforeLivestream { get; }
    }
}
