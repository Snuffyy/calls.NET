using ite4160.Data;
using ite4160.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ite4160.Pages
{
    public class CallDetailsModel : PageModel
    {

        private readonly EventContext _context;

        public CallDetailsModel(EventContext context)
        {
            _context = context;
        }

        public int Id { get; set; }
        public IList<Event> Events { get; set; }
        public Call Call { get; set; }

        public async Task OnGetAsync(int id)
        {
            Id = id;

            Events = await _context.Events
            .Include(e => e.Call)
            .Where(e => e.Call.ID == id)
            .OrderBy(e => e.Type)
            .ToListAsync();

            Call = Events.First().Call;
        }
    }
}