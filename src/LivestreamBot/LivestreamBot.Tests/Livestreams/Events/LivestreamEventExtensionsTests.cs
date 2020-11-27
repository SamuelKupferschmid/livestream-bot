using System;

using FluentAssertions;

using LivestreamBot.Livestream.Events;

using Xunit;

namespace LivestreamBot.Tests.Livestreams.Events
{
    public class LivestreamEventExtensionsTests
    {
        [Theory]
        [InlineData(18, 20, DayOfWeek.Friday, true, "during event")] 
        [InlineData(13, 15, DayOfWeek.Friday, false, "earlier that day")]
        [InlineData(22, 23, DayOfWeek.Friday, false, "later that day")]
        [InlineData(19, 23, DayOfWeek.Friday, false, "at the beginning")]
        [InlineData(16, 19, DayOfWeek.Friday, false, "at the end")]
        [InlineData(17, 23, DayOfWeek.Wednesday, false, "earlier that week")]
        [InlineData(17, 23, DayOfWeek.Saturday, false, "later that week")]
        public void GetLivestreamEventInfo_DetectOngoing(int livestreamStartHour, int livesteamEndHour, DayOfWeek dayOfWeek, bool isOngoing, string description)
        {
            var friday19_00 = new DateTime(2020, 11, 20, 19, 00, 00);
            var testee = new LivestreamEvent
            {
                DayOfWeek = dayOfWeek,
                LivestreamEventStart = TimeSpan.FromHours(livestreamStartHour),
                LivestreamEventEnd = TimeSpan.FromHours(livesteamEndHour),
            };

            testee.GetLivestreamEventInfo(friday19_00).isOngoing.Should().Be(isOngoing, description);
        }


        [Theory]
        [InlineData(18, 30, DayOfWeek.Friday, 60, "1 hour before")]
        [InlineData(19, 30, DayOfWeek.Friday, 0, "at the beginning")]
        [InlineData(19, 30, DayOfWeek.Saturday, 6 * 24 * 60, "1 day later")]
        [InlineData(19, 30, DayOfWeek.Thursday, 24 * 60, "1 day earlier")]
        public void GetLivestreamEventInfo_CalculateUntilNext(int currentHour, int currentMin, DayOfWeek dayOfWeek, int minutesUntilNext, string description)
        {
            var sunday = new DateTime(2020, 11, 22, currentHour, currentMin, 00);
            var targetDateTime = sunday.AddDays((int)dayOfWeek);

            var friday19_30Event = new LivestreamEvent
            {
                DayOfWeek = DayOfWeek.Friday,
                LivestreamEventStart = new TimeSpan(19,30,00),
            };

            friday19_30Event.GetLivestreamEventInfo(targetDateTime).untilNext.TotalMinutes.Should().Be(minutesUntilNext, description);
        }
    }
}
