using System;
using System.Linq;

namespace ite4160.Models
{
    public class NumberCall
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public string Receiver { get; set; }
        public CallType Type { get; set; }

        public NumberCall(Call call)
        {
            Timestamp = CallTimestamp(call);
            Duration = CallDuration(call);
            Receiver = call.Receiver;
            Type = call.Type;
        }
        public NumberCall(DateTime timestamp, TimeSpan duration, string receiver, CallType type)
        {
            Timestamp = timestamp;
            Duration = duration;
            Receiver = receiver;
            Type = type;
        }

        private DateTime CallTimestamp(Call call)
        {
            return call.Events.Single(e => e.Type == EventType.PickUp).Timestamp;
        }

        private TimeSpan CallDuration(Call call)
        {
            if (call.Type != CallType.Full) return TimeSpan.Zero;

            var established = call.Events.First(e => e.Type == EventType.CallEstablished).Timestamp;
            var end = call.Events.First(e => e.Type == EventType.CallEnd).Timestamp;

            return end.Subtract(established);
        }
    }
}