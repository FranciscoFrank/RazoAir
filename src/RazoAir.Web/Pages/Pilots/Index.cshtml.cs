using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Pilots;

public class IndexModel(RazoAirDbContext db) : PageModel
{
    public List<Pilot> Pilots { get; set; } = [];

    public async Task OnGetAsync()
    {
        Pilots = await db.Pilots
            .AsNoTracking()
            .Include(p => p.Airline)
            .OrderByDescending(p => p.FlightHours)
            .ToListAsync();
    }
}
