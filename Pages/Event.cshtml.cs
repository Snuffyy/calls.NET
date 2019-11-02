using System;
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

        public ISet<EventType> EventTypes { get; set; } = new HashSet<EventType>();

        public async Task OnGetAsync(int? pageIndex, int? pageSize, string sort, string filter, string eventType)
        {
            PageSize = pageSize ?? PageSize;
            CallerSort = ReverseCallerSortQueryParam(sort);
            ReceiverSort = ReverseReceiverSortQueryParam(sort);
            CurrentSort = sort;
            CurrentFilter = filter;

            var events = FindEvents(sort, filter, eventType);
            //Events = await PaginatedList<Event>.CreateAsync(events, pageIndex ?? 1, PageSize);
            Events = PaginatedList<Event>.Create(events, pageIndex ?? 1, PageSize);
        }

        private IEnumerable<Event> FindEvents(string sort, string filter, string eventType)
        {
            var events = _context.Events;
            var filteredByNumber = FilterEventsByNumber(events, filter);
            var sorted = SortEvents(filteredByNumber, sort);
            var filteredByType = FilterEventsByType(sorted, eventType);


            return filteredByType;
        }

        private IQueryable<Event> SortEvents(IQueryable<Event> events, string sort)
        {
            switch (sort)
            {
                case "caller_desc":
                    return events.Include(e => e.Call).OrderByDescending(e => e.Call.Caller);
                case "receiver_desc":
                    return events.Include(e => e.Call).OrderByDescending(e => e.Call.Receiver);
                case "receiver_asc":
                    return events.Include(e => e.Call).OrderBy(e => e.Call.Receiver);
                case "caller_asc":
                    return events.Include(e => e.Call).OrderBy(e => e.Call.Caller);
                default:
                    return events.Include(e => e.Call).OrderBy(e => e.Timestamp);
            }
        }

        private IQueryable<Event> FilterEventsByNumber(IQueryable<Event> events, string filter)
        {
            if (filter == null) return events;

            return events.Where(e => e.Call.Caller.Contains(filter) || (e.Call.Receiver != null && e.Call.Receiver.Contains(filter)));
        }

        public IEnumerable<Event> FilterEventsByType(IQueryable<Event> events, string eventType)
        {
            EventType? type = DecodeEventType(eventType);

            if (!type.HasValue) return events;

            if (!EventTypes.Contains(type.Value)) EventTypes.Add(type.Value);

            return events
                .AsEnumerable()
                .Where(e => EventTypes.Contains(e.Type));
        }

        private EventType? DecodeEventType(string eventType)
        {
            switch (eventType)
            {
                case "pickup":
                    return EventType.PickUp;
                case "dialling":
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
