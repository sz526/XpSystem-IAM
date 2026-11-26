# XpSystem — Containerized XP API

A small XP/leveling Web API, containerized with Docker, backed by PostgreSQL,
and exposed through a Traefik ingress.

## Architecture

- **XpSystem** (Core): `PlayerModel`, `IPlayerRepository`, `FakePlayerRepository`,
  `PlayerService`. The service depends only on the repository interface — it does
  not know about EF Core or PostgreSQL.
- **XpSystem.Api**: controllers, request DTOs, `XpSystemDbContext`,
  `EfPlayerRepository`, and the Scalar API reference.
- **XpSystem.Tests**: xUnit tests that use the in-memory `FakePlayerRepository`.

## Run

```powershell
docker compose up --build
```

| Entry | URL |
|---|---|
| API health | http://api.localhost/health |
| Scalar UI | http://api.localhost/scalar/v1 |
| pgAdmin | http://pgadmin.localhost (`admin@admin.com` / `admin`) |
| Traefik dashboard | http://localhost:8081/dashboard/ |

Direct host ports (kept for debugging): API `8080`, pgAdmin `5051`, PostgreSQL `5433`.

## Data

- PostgreSQL database `xpdb`, persisted in the `pgdata` volume.
- EF Core migrations are applied automatically at API startup.
- The Fake repository is used only by unit tests; the running API uses the EF Core repository.

## Tests

```powershell
dotnet test
```





The container cannot resolve `*.localhost` (the resolver maps it to loopback), so instead of a lazy `MetadataAddress` fetch we load the JWKS once at startup from the internal service URL and validate issuer/audience ourselves.