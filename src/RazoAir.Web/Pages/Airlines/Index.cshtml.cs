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
        var airlines = await db.Airlines.OrderBy(a => a.Name).ToListAsync();

        Rows = [];
        foreach (var airline in airlines)
        {
            var pilotCount = await db.Pilots.CountAsync(p => p.AirlineId == airline.Id);
            var flightCount = await db.Flights.CountAsync(f => f.AirlineId == airline.Id && f.DepartureTime >= DateTime.Today);
            Rows.Add(new AirlineRow(airline, pilotCount, flightCount));
        }
    }
}
