# ParkinsonAlarm

.NET 8 Web API for raising alarms when a Parkinson patient is detected attempting to walk alone or falling.

Quick start:
1. Set your SQL Server connection string in `appsettings.json` (ConnectionStrings:DefaultConnection).
2. Install EF tool if needed: __dotnet tool install --global dotnet-ef__
3. Create migration: __dotnet ef migrations add InitialCreate__
4. Apply migration: __dotnet ef database update__
5. Run: __dotnet run__
6. Swagger UI: https://localhost:{port}/swagger

Angular notes:
- Install SignalR client: `npm install @microsoft/signalr`
- Use `http://localhost:4200` as `AllowedOrigins` or adjust accordingly.
- SignalR Hub endpoint: `/alarmsHub` (client listens to `ReceiveAlarm` and `AlarmAcknowledged`).

Security:
- Use environment variables or secret store for production connection strings.
- Add authentication (JWT) before exposing public endpoints.