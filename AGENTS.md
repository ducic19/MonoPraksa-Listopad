# AGENTS.md

Gym membership app: .NET 10 Web API in `webap/`, React 19 + Vite SPA in `frontend/my-app/`.
All code comments/UI strings are Croatian; keep that style.

## Layout

- `webap/Gymap.sln` — API split into `Gymap` (entrypoint, controllers), `Gymap.Service`, `Gymap.Repository` (EF `AppDbContext` + migrations), `Gymap.Model`, `DTO`, `Gymap.{Service,Repository}.Common` (interfaces), `Gymap.Middleware`, `Gymap.Constants`, `Gymap.Common`.
- `frontend/my-app/` — the real SPA (`/`, `/login`, `/search` routes in `src/main.jsx`).
- `frontend/package.json` — stray duplicate that only lists `react-router-dom`. Always run npm from `frontend/my-app`.
- `konzolnaap/` — standalone `net10.0` console exercise (`Program.cs`); not part of `Gymap.sln`.
- `monop.sql` — school-exercise schema. It does **not** match the EF model (no `PasswordHash`/`Role`, `subs_id NOT NULL`); loading it breaks auth. Don't use it to bootstrap the API.

## Commands

```bash
cd webap && dotnet build Gymap.sln          # there is no root solution
cd webap/Gymap && dotnet run --launch-profile http   # http://localhost:5283, Swagger /swagger

cd frontend/my-app && npm install           # required: node_modules currently lacks vite/eslint
npm run dev                                 # proxies /api -> http://localhost:5283
npm run lint && npm run build
```

- No test projects exist anywhere. `dotnet build` + starting the API + `npm run lint` is the full verification surface.
- `dotnet build` emits EF Core 10.0.4-vs-10.0.12 assembly-conflict warnings (`MSB3277`) from the mixed Npgsql/EF package versions; build still succeeds.

## Configuration

- `appsettings*.json` contain **no** connection string and no `JwtSettings`; they come from user secrets (`UserSecretsId d0f2a260-04c1-4f7a-a9a9-ec4ec08ca86e`, set with `dotnet user-secrets` in `webap/Gymap`). Required keys: `ConnectionStrings:PostgreSQLConnection` (falls back to `DefaultConnection`) and `JwtSettings:Key|Issuer|Audience`. Startup throws without them.
- Database is PostgreSQL (`UseNpgsql`, `pgcrypto`/`gen_random_uuid()`), even though `webap/Gymap/gymap.csproj` still references `Microsoft.EntityFrameworkCore.SqlServer`. Npgsql arrives transitively via `Gymap.Repository`; don't add SqlServer config.
- Schema is created by `dbContext.Database.EnsureCreated()` in `webap/Gymap/Program.cs` from the current model. The single migration (`Gymap.Repository/Migrations/*RoleColumn.cs`, namespace `WebApplication3.Repository.Migrations`) is never applied at runtime.

## Quirks worth knowing

- `Member` is declared in the **global namespace** (no namespace in `Gymap.Model/Member.cs`), while `Trainer`/`Subscription` are `gymap.Model`. EF entity names and snapshot depend on this.
- `Gymap.Service/AuthService.cs` declares `namespace gymap.DTO`; `AuthController` compensates with `using gymap.DTO;`. Several namespaces differ from project `RootNamespace`.
- `Gymap.Common` (`IIdGenerator`, `IRequestCounter`, `ILoggerNotifier` + impls) is unused scaffolding — nothing registers or calls it.
- Auth: JWT bearer (`/api/auth/register`, `/api/auth/login`); `/api/member/*` needs a token, `POST /api/member` needs role `Admin` (`gymap.Constants.Roles`).
- `LoggingMW` writes `audit_log.txt` into the API process CWD for every successful non-GET request; the file isn't gitignored.
- Member search returns raw `Member` entities (`memId`, `name`, `email`, `role`, `subsId`) — the frontend's `activeOffer`/`id` fields never come from the API. Registration also ignores the frontend's `selectedOffer`, and `AuthResponseDto.ActiveOffer` is never populated.
- Frontend calls are relative (`api/...` in `App.jsx`, `/api/...` in `Login.jsx`/`Search.jsx`) and rely on the Vite proxy; `VITE_API_URL` in `frontend/my-app/.env` is read nowhere (the file is untracked).
- Login/register and member search exist twice: inline in `App.jsx` (route `/`) and in `Login.jsx`/`Search.jsx` (`/login`, `/search`). Check which copy is the target before editing.
