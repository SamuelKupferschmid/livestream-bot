using System;
using System.Collections.Generic;

namespace LivestreamBot.Livestream.Events
{
    public interface ILivestreamEventProvider
    {
        IEnumerable<LivestreamEvent> GetWeeklyEvents();
    }
    public class LivestreamEventProvider : ILivestreamEventProvider
    {
        private readonly LivestreamEvent morningService;
        private readonly LivestreamEvent eveningService;

        public LivestreamEventProvider()
        {
            morningService = new LivestreamEvent
            {
                Identifier = "Morgen",
                DayOfWeek = DayOfWeek.Sunday,
                LocalEventStart = new TimeSpan(10, 30, 00),
                LivestreamEventStart = new TimeSpan(10, 45, 00),
                LivestreamEventEnd = new TimeSpan(11, 45, 00),
                LivestreamAnnouncmentLead = new TimeSpan(04, 00, 00),
            };

            eveningService = new LivestreamEvent
            {
                Identifier = "Abend",
                DayOfWeek = DayOfWeek.Sunday,
                LocalEventStart = new TimeSpan(19, 00, 00),
                LivestreamEventStart = new TimeSpan(19, 15, 00),
                LivestreamEventEnd = new TimeSpan(20, 15, 00),
                LivestreamAnnouncmentLead = new TimeSpan(04, 00, 00),
        };
        }

        public IEnumerable<LivestreamEvent> GetWeeklyEvents()
        {
            yield return this.morningService;
            yield return this.eveningService;
        }
    }
}
