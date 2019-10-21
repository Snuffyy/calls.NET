using System;
using System.Collections.Generic;
using ite4160.Models;
using ite4160.Util;

namespace ite4160.Data.Provider
{
    public class EventProvider
    {
        public IEnumerable<Event> EventsForCall(Call call)
        {
            switch (call.Type)
            {
                case CallType.Full:
                    return Events(call, EventType.PickUp, EventType.Dial, EventType.CallEstablished, EventType.CallEnd, EventType.HangUp);
                case CallType.Cancelled:
                    return Events(call, EventType.PickUp, EventType.Dial, EventType.CallEnd);
                case CallType.NonDialled:
                    return Events(call, EventType.PickUp, EventType.HangUp);
            }

            throw new ArgumentException("Unsupported call type");
        }

        private IEnumerable<Event> Events(Call call, params EventType[] types)
        {
            var events = new List<Event>();

            foreach (var type in types) events.Add(Event(call, type));

            return events;
        }

        private Event Event(Call call, EventType type)
        {
            return new Event()
            {
                Type = type,
                Timestamp = TimestampFormatter.RoundTimestampToSeconds(DateTime.Now),
                Call = call
            };
        }


    }
}