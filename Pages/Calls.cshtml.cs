using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ite4160.Data;
using ite4160.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ite4160.Pages
{
    public class CallsModel : PageModel
    {
        private readonly EventContext _context;

        public CallsModel(EventContext context)
        {
            _context = context;
        }

        public string Number { get; set; }
        public IList<Event> Events { get; set; }
        public IList<NumberCall> NumberCalls { get; set; }

        public async Task OnGetAsync(string number)
        {
            Number = number;

            List<NumberEvent> numberEvents = await _context.Events
                .Where(e => e.Call.Caller == number)
                .Join(
                    _context.Calls,
                    e => e.Call.ID,
                    c => c.ID,
                    (e, call) =>
                    new NumberEvent()
                    {
                        CallId = call.ID,
                        Event = e,
                        Receiver = call.Receiver,
                        CallType = call.Type
                    }
                )
                .ToListAsync();


            // NumberCalls = numberEvents
            //     .GroupBy(ne => ne.CallId)
            //     .Select(ne => ne.ToList())
            //     .Select(ne =>
            //     new NumberCall()
            //     {
            //         Timestamp = CallTimestamp(ne),
            //         Duration = CallDuration(ne),
            //         Receiver = CallReceiver(ne),
            //         Type = CallType(ne)
            //     })
            //     .ToList();


            // var calls = await _context.Calls
            //     .Where(c => c.Caller == number)
            //     .Join(
            //         _context.Events,
            //         c => c.ID,
            //         e => e.Call.ID,
            //         (call, e) =>
            //         new
            //         {
            //             Events = e,
            //             Receiver = call.Receiver,
            //             Type = call.Type
            //         }
            //     )
            //     .ToListAsync();
        }



        private DateTime? CallTimestamp(IList<NumberEvent> numberEvents)
        {

            foreach (var ne in numberEvents)
            {
                if (ne.Event.Type == EventType.PickUp) return ne.Event.Timestamp;
            }

            return null;
        }

        private TimeSpan CallDuration(IList<NumberEvent> numberEvents)
        {
            throw new NotImplementedException();
        }

        private string CallReceiver(List<NumberEvent> ne)
        {
            throw new NotImplementedException();
        }

        private CallType CallType(List<NumberEvent> ne)
        {
            throw new NotImplementedException();
        }
    }
}