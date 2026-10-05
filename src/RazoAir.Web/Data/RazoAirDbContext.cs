using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RazoAir.Web.Models;

namespace RazoAir.Web.Data;

public class RazoAirDbContext(DbContextOptions<RazoAirDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Airline> Airlines => Set<Airline>();
    public DbSet<Aircraft> Aircraft => Set<Aircraft>();
    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Pilot> Pilots => Set<Pilot>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Airport>()
            .HasIndex(a => a.IataCode)
            .IsUnique();

        modelBuilder.Entity<Airline>()
            .HasIndex(a => a.IataCode)
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new { s.AircraftId, s.Row, s.Letter })
            .IsUnique();

        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.BookingReference)
            .IsUnique();

        // A seat can only be booked once per flight.
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => new { t.FlightId, t.SeatId })
            .IsUnique();

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.DepartureAirport)
            .WithMany(a => a.DeparturesFrom)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.ArrivalAirport)
            .WithMany(a => a.ArrivalsTo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.Captain)
            .WithMany(p => p.FlightsAsCaptain)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.FirstOfficer)
            .WithMany(p => p.FlightsAsFirstOfficer)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Flight>()
            .HasIndex(f => new { f.DepartureAirportId, f.DepartureTime });

        modelBuilder.Entity<Flight>()
            .HasIndex(f => new { f.DepartureAirportId, f.ArrivalAirportId, f.DepartureTime });

        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.PassengerEmail);
    }
}
