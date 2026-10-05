using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages;

public class IndexModel(RazoAirDbContext db) : PageModel
{
    public List<Airport> Airports { get; set; } = [];
    public List<Airline> Airlines { get; set; } = [];
    public int FlightCount { get; set; }
    public int DestinationCount { get; set; }

    public async Task OnGetAsync()
    {
        Airports = await db.Airports.AsNoTracking().OrderBy(a => a.City).ToListAsync();
        Airlines = await db.Airlines.AsNoTracking().OrderBy(a => a.Name).ToListAsync();
        FlightCount = await db.Flights.CountAsync();
        DestinationCount = Airports.Count;
    }
}
