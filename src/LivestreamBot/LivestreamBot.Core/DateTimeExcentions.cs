using System;
using System.Globalization;

namespace LivestreamBot.Core
{
    public static class DateTimeExcentions
    {
        public static string ToRfc3339String(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        }
    }
}
