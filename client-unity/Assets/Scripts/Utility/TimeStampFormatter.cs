using SpacetimeDB;
using System;

public static class TimestampFormatter
{
    private const long UnixEpochTicks = 621355968000000000; // Jan 1 1970 in .NET ticks

    public static string Format(Timestamp ts)
    {
        long microseconds = ts.MicrosecondsSinceUnixEpoch;
        long ticks = microseconds * 10; // .NET ticks = 100ns

        DateTime sent = new DateTime(UnixEpochTicks + ticks, DateTimeKind.Utc).ToLocalTime();

        DateTime now = DateTime.Now;
        DateTime today = now.Date;
        DateTime sentDay = sent.Date;

        if (sentDay == today)
        {
            return sent.ToString("hh:mm tt"); // Today: only time
        }
        else if (sentDay == today.AddDays(-1))
        {
            return $"Yesterday {sent:hh:mm tt}";
        }
        else if (sent.Year == now.Year)
        {
            return sent.ToString("MMM dd HH:mm"); // Same year: month, day, time
        }
        else
        {
            return sent.ToString("yyyy-MM-dd HH:mm"); // Different year
        }
    }
}
