
using System;
using System.Collections.Generic;

namespace LivestreamBot.Livestream
{
    public interface IRecuringEventsProvider
    {
        IEnumerable<WeeklyEvent> GetWeeklyEvents();
    }

    public class RecuringEventsProvider : IRecuringEventsProvider
    {
        public IEnumerable<WeeklyEvent> GetWeeklyEvents()
        {
            yield return new WeeklyEvent
            {
                DayOfWeek = DayOfWeek.Sunday,
                StartTime = new TimeSpan(10, 45, 00),
                EndTime = new TimeSpan(12, 00, 00),
            };

            yield return new WeeklyEvent
            {
                DayOfWeek = DayOfWeek.Sunday,
                StartTime = new TimeSpan(19, 15, 00),
                EndTime = new TimeSpan(20, 30, 00)
            };
        }
    }

    public class WeeklyEvent
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class RecuringEventInfo
    {
        public DateTime? NextStart { get; set; }
        public bool IsOngoing { get; set; }
        public DateTime? CurrentStart { get; set; }
        public DateTime? CurrentEnd { get; set; }
        public DateTime? PreviousEnd { get; set; }
    }
}
