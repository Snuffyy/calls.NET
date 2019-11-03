using System;

namespace ite4160.Models
{
    public class NumberCall
    {
        public DateTime Timestamp { get; set; }
        public TimeSpan Duration { get; set; }
        public string Receiver { get; set; }
        public CallType Type { get; set; }

        // public NumberCall(DateTime timestamp, TimeSpan duration, string receiver, CallType type)
        // {
        //     Timestamp = timestamp;
        //     Duration = duration;
        //     Receiver = receiver;
        //     Type = type;
        // }
    }
}