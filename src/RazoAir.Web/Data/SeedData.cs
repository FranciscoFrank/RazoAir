using RazoAir.Web.Models;

namespace RazoAir.Web.Data;

public static class SeedData
{
    public static void Initialize(RazoAirDbContext db)
    {
        if (db.Flights.Any())
        {
            return;
        }

        var airports = new[]
        {
            new Airport { IataCode = "KBP", Name = "Boryspil", City = "Kyiv", Country = "Ukraine" },
            new Airport { IataCode = "LWO", Name = "Danylo Halytskyi", City = "Lviv", Country = "Ukraine" },
            new Airport { IataCode = "ODS", Name = "Odesa", City = "Odesa", Country = "Ukraine" },
            new Airport { IataCode = "WAW", Name = "Chopin", City = "Warsaw", Country = "Poland" },
            new Airport { IataCode = "VIE", Name = "Schwechat", City = "Vienna", Country = "Austria" },
            new Airport { IataCode = "IST", Name = "Istanbul", City = "Istanbul", Country = "Turkey" },
            new Airport { IataCode = "FRA", Name = "Frankfurt am Main", City = "Frankfurt", Country = "Germany" },
            new Airport { IataCode = "CDG", Name = "Charles de Gaulle", City = "Paris", Country = "France" },
            new Airport { IataCode = "AMS", Name = "Schiphol", City = "Amsterdam", Country = "Netherlands" },
            new Airport { IataCode = "BER", Name = "Brandenburg", City = "Berlin", Country = "Germany" },
        };
        db.Airports.AddRange(airports);

        var razoAir = new Airline { IataCode = "RA", Name = "RazoAir", AccentColor = "#5B4B7A", Description = "The platform's flagship carrier — comfortable flights with a focus on punctuality." };
        var skyBridge = new Airline { IataCode = "SB", Name = "SkyBridge Airlines", AccentColor = "#3B6E8F", Description = "A regional carrier with a wide network of short-haul routes." };
        var nordWing = new Airline { IataCode = "NW", Name = "NordWing", AccentColor = "#2F7D5A", Description = "Budget flights without the extra costs." };
        var aeroVista = new Airline { IataCode = "AV", Name = "AeroVista", AccentColor = "#A15C3E", Description = "Premium service on popular European routes." };
        var airlines = new[] { razoAir, skyBridge, nordWing, aeroVista };
        db.Airlines.AddRange(airlines);

        var a320 = new Aircraft { Model = "A320neo", Manufacturer = "Airbus", Rows = 30, SeatLayout = "ABC DEF", BusinessRows = 4 };
        var b738 = new Aircraft { Model = "737-800", Manufacturer = "Boeing", Rows = 32, SeatLayout = "ABC DEF", BusinessRows = 3 };
        var e195 = new Aircraft { Model = "E195-E2", Manufacturer = "Embraer", Rows = 25, SeatLayout = "AB CD", BusinessRows = 2 };
        var aircraftTypes = new[] { a320, b738, e195 };
        db.Aircraft.AddRange(aircraftTypes);

        foreach (var ac in aircraftTypes)
        {
            var columns = ac.SeatLayout.Where(c => c != ' ').ToArray();
            for (var row = 1; row <= ac.Rows; row++)
            {
                var seatClass = row <= ac.BusinessRows ? SeatClass.Business : SeatClass.Economy;
                for (var i = 0; i < columns.Length; i++)
                {
                    ac.Seats.Add(new Seat
                    {
                        Aircraft = ac,
                        Row = row,
                        Letter = columns[i].ToString(),
                        SeatClass = seatClass,
                        IsWindow = i == 0 || i == columns.Length - 1,
                        IsAisle = i == columns.Length / 2 - 1 || i == columns.Length / 2,
                    });
                }
            }
        }

        var pilotNames = new (string Name, string Bio)[]
        {
            ("Oleksandr Kovalchuk", "Over 15 years at the controls of long-haul and regional airliners."),
            ("Maria Hnatiuk", "One of the company's first female captains, specializing in challenging weather conditions."),
            ("Ihor Savchuk", "Flight simulator instructor who has trained more than 40 first officers."),
            ("Nataliya Bondarenko", "Came from military aviation, has logged thousands of hours flying at night."),
            ("Dmytro Lytvyn", "Specialist in short European routes, known for flawless punctuality."),
            ("Tetiana Romaniuk", "First officer preparing for captain certification next year."),
            ("Andriy Melnyk", "Company veteran who has spent most of his career on the Airbus A320."),
            ("Yuliia Tkachenko", "Passionate about aviation meteorology, lectures junior pilots."),
            ("Bohdan Kravets", "Moved over from cargo aviation, values procedural precision."),
            ("Sofiia Pavlenko", "The company's youngest captain, earned her rating at age 27."),
            ("Viktor Havryliuk", "Has flown the Boeing 737 for over a decade, mentors new first officers."),
            ("Olena Zakharchuk", "Specializes in flights into airports with challenging mountain terrain."),
        };

        var rng = new Random(2026_09_15);
        var pilots = new List<Pilot>();
        for (var i = 0; i < pilotNames.Length; i++)
        {
            var (name, bio) = pilotNames[i];
            var airline = airlines[i % airlines.Length];
            var isCaptain = i % 3 != 2;
            pilots.Add(new Pilot
            {
                FullName = name,
                Airline = airline,
                Rank = isCaptain ? (i % 5 == 0 ? PilotRank.SeniorCaptain : PilotRank.Captain) : PilotRank.FirstOfficer,
                FlightHours = isCaptain ? rng.Next(6000, 16000) : rng.Next(800, 4000),
                YearsOfExperience = isCaptain ? rng.Next(8, 25) : rng.Next(1, 6),
                Bio = bio,
                Certifications = isCaptain ? "ATPL, Type Rating A320/B737" : "CPL, Type Rating A320",
            });
        }
        db.Pilots.AddRange(pilots);

        var kbp = airports[0];
        var lwo = airports[1];
        var ods = airports[2];
        var waw = airports[3];
        var vie = airports[4];
        var ist = airports[5];
        var fra = airports[6];
        var cdg = airports[7];
        var ams = airports[8];
        var ber = airports[9];

        // Routes with duration in minutes; some routes are intentionally served by several
        // carriers on the same day/time so travellers can pick between offers.
        var routes = new (Airport From, Airport To, int DurationMin)[]
        {
            (kbp, waw, 105), (kbp, vie, 130), (kbp, ist, 150), (kbp, fra, 175), (kbp, cdg, 195),
            (lwo, waw, 60),  (lwo, vie, 85),
            (ods, ist, 100), (ods, vie, 140),
            (kbp, ams, 190), (kbp, ber, 145),
            (lwo, fra, 120), (ods, waw, 95),
        };

        var today = DateTime.Today;
        var flights = new List<Flight>();
        var flightNoCounter = 100;

        Pilot PickCaptain(Airline airline) =>
            pilots.Where(p => p.Airline == airline && p.Rank != PilotRank.FirstOfficer).OrderBy(_ => rng.Next()).FirstOrDefault()
            ?? pilots.First(p => p.Airline == airline);

        Pilot PickFirstOfficer(Airline airline) =>
            pilots.Where(p => p.Airline == airline && p.Rank == PilotRank.FirstOfficer).OrderBy(_ => rng.Next()).FirstOrDefault()
            ?? pilots.First(p => p.Airline == airline);

        Aircraft PickAircraft() => aircraftTypes[rng.Next(aircraftTypes.Length)];

        for (var dayOffset = 1; dayOffset <= 21; dayOffset++)
        {
            var date = today.AddDays(dayOffset);

            foreach (var (from, to, durationMin) in routes)
            {
                // Every route is flown by RazoAir daily, plus 1-2 competitors on select
                // weekdays so the same date/time/airport can offer multiple carrier choices.
                var hour = 6 + rng.Next(0, 14);
                var minute = rng.Next(0, 4) * 15;
                var departure = date.AddHours(hour).AddMinutes(minute);

                var carriersToday = new List<Airline> { razoAir };
                if (dayOffset % 2 == 0) carriersToday.Add(skyBridge);
                if (dayOffset % 3 == 0) carriersToday.Add(nordWing);
                if (dayOffset % 4 == 0) carriersToday.Add(aeroVista);

                foreach (var airline in carriersToday)
                {
                    // Competing carriers on the same route/day depart within the same
                    // ~30 minute window as RazoAir so travellers see a genuine side-by-side choice.
                    var offsetMinutes = airline == razoAir ? 0 : rng.Next(-20, 25);
                    var dep = departure.AddMinutes(offsetMinutes);
                    var aircraft = PickAircraft();
                    var basePrice = 800 + durationMin * 3.2m + rng.Next(-150, 300);

                    flights.Add(new Flight
                    {
                        FlightNumber = $"{airline.IataCode}{flightNoCounter++}",
                        Airline = airline,
                        Aircraft = aircraft,
                        DepartureAirport = from,
                        ArrivalAirport = to,
                        DepartureTime = dep,
                        ArrivalTime = dep.AddMinutes(durationMin),
                        Captain = PickCaptain(airline),
                        FirstOfficer = PickFirstOfficer(airline),
                        EconomyPrice = Math.Round(basePrice, 0),
                        BusinessPrice = Math.Round(basePrice * 2.6m, 0),
                    });
                }
            }
        }

        db.Flights.AddRange(flights);
        db.SaveChanges();
    }
}
