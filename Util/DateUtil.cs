using System;
using System.Globalization;

namespace ite4160.Util
{
    public static class TimestampFormatter
    {
        public static readonly string Format = "dd/MM/yyyy hh:mm:ss";

        private static string TimestampToString(DateTime timestamp) => timestamp.ToString(Format);

        public static DateTime RoundTimestampToSeconds(DateTime timestamp) => DateTime.ParseExact(TimestampToString(timestamp), Format, CultureInfo.InvariantCulture);

    }
}