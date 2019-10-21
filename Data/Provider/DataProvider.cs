using System;
using System.Collections.Generic;
using System.Linq;
using ite4160.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static ite4160.Models.Call;

namespace ite4160.Data.Provider
{

    public class DataProvider
    {
        private int Full { get; set; }
        private int Cancelled { get; set; }
        private int NonDialled { get; set; }

        private CallProvider callProvider = new CallProvider();
        private EventProvider eventProvider = new EventProvider();

        public DataProvider(int percentFull, int percentCancelled, int percentNonDialled)
        {
            Full = percentFull;
            Cancelled = percentCancelled;
            NonDialled = percentNonDialled;
        }


        public void Provide(int total, IServiceProvider serviceProvider)
        {
            Full = FromPercent(total, Full);
            Cancelled = FromPercent(total, Cancelled);
            NonDialled = FromPercent(total, NonDialled);

            using
            (
                var context = new EventContext(
                    serviceProvider.GetRequiredService<DbContextOptions<EventContext>>()
                )
            )
            {

                if (context.Calls.Any() || context.Events.Any())
                {
                    return;
                }

                // TODO: .And
                var callsFull = callProvider.Calls(Full, CallType.Full);
                var callsCancelled = callProvider.Calls(Cancelled, CallType.Cancelled);
                var callsNonDialled = callProvider.Calls(NonDialled, CallType.NonDialled);

                var calls = Enumerable.Concat(callsFull, callsCancelled).Concat(callsNonDialled);
                var events = new List<Event>();

                foreach (var call in calls)
                {
                    events.AddRange(eventProvider.EventsForCall(call));
                }


                context.Events.AddRange(events);

                context.SaveChanges();
            }
        }

        private int FromPercent(int total, int percent) => total * percent / 100;
    }

}