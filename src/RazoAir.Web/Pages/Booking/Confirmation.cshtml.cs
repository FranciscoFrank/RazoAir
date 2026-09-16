using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Data;
using RazoAir.Web.Models;

namespace RazoAir.Web.Pages.Booking;

public class ConfirmationModel(RazoAirDbContext db) : PageModel
{
    public Ticket Ticket { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string reference)
    {
        var ticket = await db.Tickets
            .Include(t => t.Seat)
            .Include(t => t.Flight).ThenInclude(f => f.Airline)
            .Include(t => t.Flight).ThenInclude(f => f.Aircraft)
            .Include(t => t.Flight).ThenInclude(f => f.DepartureAirport)
            .Include(t => t.Flight).ThenInclude(f => f.ArrivalAirport)
            .Include(t => t.Flight).ThenInclude(f => f.Captain)
            .Include(t => t.Flight).ThenInclude(f => f.FirstOfficer)
            .FirstOrDefaultAsync(t => t.BookingReference == reference);

        if (ticket is null)
        {
            return NotFound();
        }

        Ticket = ticket;
        return Page();
    }
}
