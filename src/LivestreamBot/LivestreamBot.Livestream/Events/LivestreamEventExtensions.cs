using System;

namespace LivestreamBot.Livestream.Events
{
    public static class LivestreamEventExtensions
    {
        public static (TimeSpan untilNext, bool isOngoing)  GetLivestreamEventInfo(this LivestreamEvent @event, DateTime dateTime)
        {
            var nextStart = @event.GetNext(dateTime);

            var previousStart = nextStart.AddDays(-7);

            var untilNext = nextStart - dateTime;
            var isOngoing = previousStart + @event.LivestreamEventDuration > dateTime;

            return (untilNext, isOngoing);
        }

        public static DateTime GetNext(this LivestreamEvent @event, DateTime dateTime)
        {
            var daysDiff = @event.DayOfWeek - dateTime.DayOfWeek;
            var nextStart = dateTime.Date.AddDays(daysDiff) + @event.LivestreamEventStart;

            if (nextStart < dateTime)
            {
                // earlier on the same Date
                nextStart = nextStart.AddDays(7);
            }

            return nextStart;
        }
    }
}
