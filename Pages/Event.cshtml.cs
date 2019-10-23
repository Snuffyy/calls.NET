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

        public string CurrentSort { get; set; }
        public string CallerSort { get; set; }
        public string ReceiverSort { get; set; }

        public async Task OnGetAsync(int? pageIndex, int? pageSize, string sort)
        {
            PageSize = pageSize ?? PageSize;
            CallerSort = ReverseCallerSortQueryParam(sort);
            ReceiverSort = ReverseReceiverSortQueryParam(sort);
            CurrentSort = sort;

            var events = FindEvents(sort);
            Events = await PaginatedList<Event>.CreateAsync(events, pageIndex ?? 1, PageSize);
        }

        private IQueryable<Event> FindEvents(string sort)
        {
            switch (sort)
            {
                case "caller_desc":
                    return _context.Events.Include(e => e.Call).OrderByDescending(e => e.Call.Caller);
                case "receiver_desc":
                    return _context.Events.Include(e => e.Call).OrderByDescending(e => e.Call.Receiver);
                case "receiver_asc":
                    return _context.Events.Include(e => e.Call).OrderBy(e => e.Call.Receiver);
                case "caller_asc":
                    return _context.Events.Include(e => e.Call).OrderBy(e => e.Call.Caller);
                default:
                    return _context.Events.Include(e => e.Call).OrderBy(e => e.Timestamp);
            }
        }

        private string ReverseCallerSortQueryParam(string param)
        {
            if (string.IsNullOrEmpty(param)) return "caller_asc";
            else if (param.Equals("caller_asc")) return "caller_desc";
            else return "caller_asc";
        }

        private string ReverseReceiverSortQueryParam(string param)
        {
            if (string.IsNullOrEmpty(param)) return "receiver_asc";
            else if (param.Equals("receiver_asc")) return "receiver_desc";
            else return "receiver_asc";
        }
    }
}
