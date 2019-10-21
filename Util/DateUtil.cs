using System;
using System.Globalization;

namespace ite4160.Util
{
    public static class TimestampFormatter
    {
        private static readonly string Format = "DD/mm/YYYY HH:MM:ss";

        private static string TimestampToString(DateTime timestamp) => timestamp.ToString(Format);

        public static DateTime RoundTimestampToSeconds(DateTime timestamp) => DateTime.ParseExact(TimestampToString(timestamp), Format, CultureInfo.InvariantCulture);

    }
}