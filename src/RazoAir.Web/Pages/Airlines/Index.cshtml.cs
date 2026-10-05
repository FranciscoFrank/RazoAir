using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Airlines;

public class IndexModel(RazoAirDbContext db) : PageModel
{
    public record AirlineRow(Airline Airline, int PilotCount, int UpcomingFlightCount);

    public List<AirlineRow> Rows { get; set; } = [];

    public async Task OnGetAsync()
    {
        var today = DateTime.Today;
        Rows = await db.Airlines
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .Select(a => new AirlineRow(
                a,
                db.Pilots.Count(p => p.AirlineId == a.Id),
                db.Flights.Count(f => f.AirlineId == a.Id && f.DepartureTime >= today)))
            .ToListAsync();
    }
}
