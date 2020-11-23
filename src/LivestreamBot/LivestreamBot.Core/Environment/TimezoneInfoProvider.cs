using System;
using System.Runtime.InteropServices;

namespace LivestreamBot.Core.Environment
{

    public class TimezoneInfoProvider
    {
        public static TimeZoneInfo GetLocalTimeZoneInfo() => TimeZoneInfo.FindSystemTimeZoneById(LocalTimezoneString);
        private static string LocalTimezoneString => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Central Europe Standard Time" : "Europe/Zurich";
    }
}
