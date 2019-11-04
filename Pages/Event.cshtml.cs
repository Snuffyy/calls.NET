using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using ite4160.Data;
using ite4160.Models;
using ite4160.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ite4160.Pages
{
    public class EventModel : PageModel
    {
        private readonly ILogger<EventModel> _logger;
        private readonly EventContext _context;

        public EventModel(ILogger<EventModel> logger, EventContext context)
        {
            _logger = logger;
            _context = context;
        }

        public PaginatedList<Event> Events { get; set; }

        public int PageSize { get; set; } = 10;

        [BindProperty(SupportsGet = true)]
        public string CurrentFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CurrentSort { get; set; }
        public string CallerSort { get; set; }
        public string ReceiverSort { get; set; }

        public ISet<EventType> EventTypes { get; } = new HashSet<EventType>();

        public async Task<IActionResult> OnGetAsync(
            bool export,
            int? pageIndex,
            int? pageSize,
            string sort,
            string filter,
            bool pickup,
            bool dial,
            bool established,
            bool end,
            bool hangup
        )
        {
            if (export)
            {
                return RedirectToPage("Event", "export", new
                {
                    sort,
                    filter,
                    pickup,
                    dial,
                    established,
                    end,
                    hangup
                });
            }

            PageSize = pageSize ?? PageSize;
            CallerSort = ReverseCallerSortQueryParam(sort);
            ReceiverSort = ReverseReceiverSortQueryParam(sort);
            CurrentSort = sort;
            CurrentFilter = filter;

            var events = FindEvents(sort, filter, pickup, dial, established, end, hangup);

            Events = await PaginatedList<Event>.CreateAsync(events, pageIndex ?? 1, PageSize);

            return Page();
        }

        public async Task<FileResult> OnGetExportAsync(
            string sort,
            string filter,
            bool pickup,
            bool dial,
            bool established,
            bool end,
            bool hangup
        )
        {
            var events = FindEvents(
                sort,
                filter,
                pickup,
                dial,
                established,
                end,
                hangup);

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer))
                {
                    var e = await events
                    .Select(e => new
                    {
                        Caller = e.Call.Caller,
                        Event = e.Type,
                        Receiver = e.Call.Receiver == null ? "" : e.Call.Receiver,
                        Timestamp = e.Timestamp.ToString(TimestampFormatter.Format)
                    })
                    .ToListAsync();

                    csv.WriteRecords(e);
                }

                return File(stream.ToArray(), "application/octet-stream", $"all_records_{DateTime.Now.ToString("yyyyMMdd")}");
            }
        }

        private IQueryable<Event> FindEvents(
            string sort,
            string filter,
            bool pickup,
            bool dial,
            bool established,
            bool end,
            bool hangup
        )
        {
            var events = _context.Events.Include(e => e.Call);

            var filteredByNumber = FilterEventsByNumber(events, filter);
            var filteredByType = FilterEventsByType(filteredByNumber, pickup, dial, established, end, hangup);
            var sorted = SortEvents(filteredByType, sort);

            return sorted;
        }

        private IQueryable<Event> SortEvents(IQueryable<Event> events, string sort)
        {
            switch (sort)
            {
                case "caller_desc":
                    return events.OrderByDescending(e => e.Call.Caller);
                case "receiver_desc":
                    return events.OrderByDescending(e => e.Call.Receiver);
                case "receiver_asc":
                    return events.OrderBy(e => e.Call.Receiver);
                case "caller_asc":
                    return events.OrderBy(e => e.Call.Caller);
                default:
                    return events.OrderBy(e => e.Timestamp);
            }
        }

        private IQueryable<Event> FilterEventsByNumber(IQueryable<Event> events, string filter)
        {
            if (filter == null) return events;

            return events.Where(e => e.Call.Caller.StartsWith(filter) || (e.Call.Receiver != null && e.Call.Receiver.StartsWith(filter)));
        }

        public IQueryable<Event> FilterEventsByType(
            IQueryable<Event> events,
            bool pickup,
            bool dial,
            bool established,
            bool end,
            bool hangup
        )
        {
            HandleEventType(EventType.PickUp, pickup);
            HandleEventType(EventType.Dial, dial);
            HandleEventType(EventType.CallEstablished, established);
            HandleEventType(EventType.CallEnd, end);
            HandleEventType(EventType.HangUp, hangup);

            return events.Where(e =>
                (!pickup && !dial && !established && !end && !hangup)
                || (pickup ? e.Type == EventType.PickUp : false)
                || (dial ? e.Type == EventType.Dial : false)
                || (established ? e.Type == EventType.CallEstablished : false)
                || (end ? e.Type == EventType.CallEnd : false)
                || (hangup ? e.Type == EventType.HangUp : false));
        }

        private void HandleEventType(EventType type, bool val)
        {
            if (val) EventTypes.Add(type);
            else EventTypes.Remove(type);
        }

        private EventType? DecodeEventType(string eventType)
        {
            switch (eventType)
            {
                case "pickup":
                    return EventType.PickUp;
                case "dial":
                    return EventType.Dial;
                case "established":
                    return EventType.CallEstablished;
                case "end":
                    return EventType.CallEnd;
                case "hangup":
                    return EventType.HangUp;
                default:
                    return null;
            }
        }

        private string ReverseCallerSortQueryParam(string param)
        {
            if (string.IsNullOrEmpty(param)) return null;
            else if (param.Equals("caller_asc")) return "caller_desc";
            else return "caller_asc";
        }

        private string ReverseReceiverSortQueryParam(string param)
        {
            if (string.IsNullOrEmpty(param)) return null;
            else if (param.Equals("receiver_asc")) return "receiver_desc";
            else return "receiver_asc";
        }
    }
}
