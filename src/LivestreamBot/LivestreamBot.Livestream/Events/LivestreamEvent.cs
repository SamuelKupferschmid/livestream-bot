using System;

namespace LivestreamBot.Livestream.Events
{

    public class LivestreamEvent
    {
        public string Identifier { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan LocalEventStart { get; set; }
        public TimeSpan LivestreamAnnouncmentLead { get; set; }
        public TimeSpan LivestreamEventStart { get; set; }
        public TimeSpan LivestreamEventEnd { get; set; }
        public TimeSpan LivestreamEventDuration => LivestreamEventEnd - LivestreamEventStart;
    }
}
