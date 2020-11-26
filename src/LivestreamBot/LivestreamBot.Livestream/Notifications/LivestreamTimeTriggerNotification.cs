
using MediatR;

using System;

namespace LivestreamBot.Livestream.Notifications
{
    public class LivestreamTimeTriggerNotification : INotification
    {
        public DateTime Last { get; set; }
        public DateTime Next { get; set; }
    }
}
