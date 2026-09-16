# RazoAir ✈️

RazoAir is an ASP.NET Core (Razor Pages) web application for browsing flights, managing airline and pilot data, and booking airline tickets.

## Features

- **Flight search & details** — browse available flights by route, date, and airline
- **Ticket booking** — create a booking, receive a confirmation, and look up an existing booking by reference number
- **Airlines & pilots directory** — browse airline and pilot information
- **User accounts** — registration, login, and profile management via ASP.NET Core Identity
- **Seeded reference data** — aircraft, airports, airlines, pilots, and flights for a ready-to-explore demo

## Tech stack

- ASP.NET Core Razor Pages
- Entity Framework Core (code-first migrations)
- ASP.NET Core Identity (authentication & authorization)

## Project structure

```
src/RazoAir.Web/
├── Data/            # DbContext, seed data, migrations, booking reference generator
├── Models/          # Domain entities (Flight, Aircraft, Airline, Pilot, Seat, Ticket, ...)
├── Pages/
│   ├── Account/     # Login, register, logout, profile management
│   ├── Airlines/    # Airline listing
│   ├── Booking/     # Create, confirm, and look up bookings
│   ├── Flights/     # Search and view flight details
│   ├── Pilots/      # Pilot listing and details
│   └── Shared/      # Layout and shared partials
└── wwwroot/         # Static assets
```

## Getting started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (see `RazoAir.Web.csproj` for the target version)
- A local database provider matching the configured connection string in `appsettings.Development.json`

### Run locally

```bash
git clone https://github.com/FranciscoFrank/RazoAir.git
cd RazoAir
dotnet restore
dotnet ef database update --project src/RazoAir.Web
dotnet run --project src/RazoAir.Web
```

The app will be available at the URL shown in the console output.

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## Security

If you discover a security vulnerability, please see [SECURITY.md](SECURITY.md) for how to report it responsibly.

## Code of Conduct

This project follows a [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you agree to abide by its terms.

## License

This project is licensed under the [MIT License](LICENSE).
