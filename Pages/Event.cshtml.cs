using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ite4160.Data;
using ite4160.Models;
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

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public string CurrentFilter { get; set; }

        public string CurrentSort { get; set; }
        public string CallerSort { get; set; }
        public string ReceiverSort { get; set; }

        public ISet<EventType> EventTypes { get; } = new HashSet<EventType>();

        public async Task OnGetAsync(
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
            PageSize = pageSize ?? PageSize;
            CallerSort = ReverseCallerSortQueryParam(sort);
            ReceiverSort = ReverseReceiverSortQueryParam(sort);
            CurrentSort = sort;
            CurrentFilter = filter;

            var events = FindEvents(sort, filter, pickup, dial, established, end, hangup);

            Events = await PaginatedList<Event>.CreateAsync(events, pageIndex ?? 1, PageSize);
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

            return events.Where(e => e.Call.Caller.Contains(filter) || (e.Call.Receiver != null && e.Call.Receiver.Contains(filter)));
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
