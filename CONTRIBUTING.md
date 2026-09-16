# Contributing to RazoAir

Thanks for your interest in contributing! This document covers how to get set up and submit changes.

## Getting started

1. Fork the repository and clone your fork locally.
2. Install the [.NET SDK](https://dotnet.microsoft.com/download) version matching `RazoAir.Web.csproj`.
3. Restore dependencies and apply migrations:
   ```bash
   dotnet restore
   dotnet ef database update --project src/RazoAir.Web
   ```
4. Run the app locally:
   ```bash
   dotnet run --project src/RazoAir.Web
   ```

## Making changes

- Create a feature branch off `main`:
  ```bash
  git checkout -b feature/short-description
  ```
- Keep commits focused and use clear, descriptive commit messages (e.g. `feat: add flight filtering by airline`).
- Follow the existing project structure — domain models in `Models/`, data access in `Data/`, and feature pages grouped under `Pages/<Feature>/`.
- If your change touches the database schema, add a matching EF Core migration:
  ```bash
  dotnet ef migrations add <DescriptiveName> --project src/RazoAir.Web
  ```

## Submitting a pull request

1. Make sure the project builds and runs without errors.
2. Push your branch and open a pull request against `main`.
3. Describe what the change does and why, and link any related issues.
4. Be responsive to review feedback — small, incremental commits during review are welcome.

## Reporting bugs and suggesting features

Please open an issue describing:

- What you expected to happen
- What actually happened (include steps to reproduce for bugs)
- Any relevant environment details (OS, .NET version)

## Code of Conduct

By participating in this project, you agree to abide by the [Code of Conduct](CODE_OF_CONDUCT.md).
