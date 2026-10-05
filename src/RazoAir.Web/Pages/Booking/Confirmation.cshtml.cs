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
        if (string.IsNullOrWhiteSpace(reference))
        {
            return NotFound();
        }

        var normalizedReference = reference.Trim().ToUpperInvariant();

        var ticket = await db.Tickets
            .AsNoTracking()
            .Include(t => t.Seat)
            .Include(t => t.Flight).ThenInclude(f => f.Airline)
            .Include(t => t.Flight).ThenInclude(f => f.Aircraft)
            .Include(t => t.Flight).ThenInclude(f => f.DepartureAirport)
            .Include(t => t.Flight).ThenInclude(f => f.ArrivalAirport)
            .Include(t => t.Flight).ThenInclude(f => f.Captain)
            .Include(t => t.Flight).ThenInclude(f => f.FirstOfficer)
            .FirstOrDefaultAsync(t => t.BookingReference == normalizedReference);

        if (ticket is null)
        {
            return NotFound();
        }

        var isAuthorized = false;

        if (TempData["AuthorizedBookingRef"] is string authorizedRef &&
            string.Equals(authorizedRef, normalizedReference, StringComparison.OrdinalIgnoreCase))
        {
            isAuthorized = true;
            TempData.Keep("AuthorizedBookingRef");
        }
        else if (User.Identity?.IsAuthenticated == true &&
                 string.Equals(User.Identity.Name, ticket.PassengerEmail, StringComparison.OrdinalIgnoreCase))
        {
            isAuthorized = true;
        }

        if (!isAuthorized)
        {
            return RedirectToPage("/Booking/Lookup", new { reference = normalizedReference });
        }

        Ticket = ticket;
        return Page();
    }
}
