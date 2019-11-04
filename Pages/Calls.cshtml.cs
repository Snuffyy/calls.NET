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

            NumberCalls = await _context.Calls
                .Where(c => c.Caller == number)
                .Include(c => c.Events)
                .Select(c => new NumberCall(c))
                .ToListAsync();
        }
    }
}