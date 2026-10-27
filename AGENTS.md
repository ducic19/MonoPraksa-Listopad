# AGENTS.md

Gym membership app: .NET 10 Web API in `webap/`, React 19 + Vite SPA in `frontend/my-app/`.
Comments in the .NET code and the API's user-facing messages are Croatian; the React SPA's UI copy is English (Croatian API messages are translated in `src/api.js`).

## Layout

- `webap/Gymap.sln` — four projects, dependency direction `Api → Service → DAL → Common`:
  - `Gymap.Api` — entrypoint (`Program.cs`), controllers, `LoggingMW`, config.
  - `Gymap.Common` — models (`Member`, `Subscription`, `Trainer`), `Roles`, DTOs in `DTO/{Members,Subscriptions,Trainers,Auth}`. No interfaces.
  - `Gymap.DAL` — `AppDbContext`, migrations, and one folder per entity (`Members/`, `Subscriptions/`, `Trainers/`) holding `I<Entity>Repository` beside `<Entity>Repository`.
  - `Gymap.Service` — one folder per entity (`Members/`, `Subscriptions/`, `Trainers/`) plus `Auth/`, each holding `I<Entity>Service` beside `<Entity>Service`; `SaveResult` is shared by the controllers.
- Every model has a repository and a service, and the interface lives in the same folder as its implementation. Adding a model means adding `Gymap.DAL/<Plural>/`, `Gymap.Service/<Plural>/`, DTOs, a controller, and two DI registrations.
- Folder/namespace names are plural (`Gymap.DAL.Members`, `Gymap.Service.Members`) so `Member` keeps resolving to the entity, not a namespace.
- `frontend/my-app/` — the SPA (`/`, `/login`, `/search` in `src/main.jsx`).
- `frontend/package.json` — stray duplicate listing only `react-router-dom`; always run npm from `frontend/my-app`.
- `konzolnaap/` — standalone `net10.0` console exercise; not part of `Gymap.sln`.
- `monop.sql` — school-exercise schema that does **not** match the EF model (no `PasswordHash`/`Role`, `subs_id NOT NULL`); don't use it to bootstrap the API.

## Commands

```bash
cd webap && dotnet build Gymap.sln
cd webap/Gymap.Api && dotnet run --launch-profile http   # http://localhost:5283, Swagger at /swagger

cd frontend/my-app && npm install   # node_modules currently lacks vite/eslint
npm run dev                         # proxies /api -> http://localhost:5283
npm run lint && npm run build
```

- No test projects exist. Verification is `dotnet build` + starting the API + `npm run lint`.
- Package versions live in `webap/Directory.Packages.props` (central package management). EF/Npgsql is pinned to 10.0.12 / 10.0.3, so the build is warning-free; don't add `Version=` attributes to `PackageReference`.

## Configuration

- `appsettings*.json` have no connection string and no `JwtSettings`; they live in user secrets (`UserSecretsId` in `Gymap.Api.csproj`, so use `dotnet user-secrets --project webap/Gymap.Api ...`). Keys: `ConnectionStrings:PostgreSQLConnection` (falls back to `DefaultConnection`) and `JwtSettings:Key|Issuer|Audience|ExpirationInMinutes`. Startup throws without them.
- PostgreSQL via `UseNpgsql` (provider comes transitively from `Gymap.DAL`; there is no SqlServer package).
- Schema is owned by EF migrations in `Gymap.DAL/Migrations`. Startup calls `dbContext.Database.Migrate()`, controlled by `Database:AutoMigrate` (default `true`).
- Any model change needs a migration, or the running app will not see the new column:
  ```bash
  cd webap
  dotnet ef migrations add <Name> --project Gymap.DAL --startup-project Gymap.Api
  dotnet ef database update   --project Gymap.DAL --startup-project Gymap.Api
  ```
- `member.email` has a real unique index (`ux_member_email`); registration duplicates are rejected both there and by an app-level pre-check.

## Auth and API behavior

- `POST /api/auth/register` → 200 with token, 409 duplicate email, 400 validation failure (password min 8).
- Emails are stored lowercase/trimmed; login returns 401 for unknown email, wrong password, or members without a password hash (admin-created rows). Roles are normalized when the token is created, so legacy lowercase `admin` rows still pass `Roles.Admin` checks.
- `/api/member/*`, `/api/subscription/*` and `/api/trainer/*` require a bearer token. `POST`, `PUT`, `DELETE` require role `Admin` (`Gymap.Common.Roles`); `GET` works for any authenticated user.
- `GET /api/member/search?name=` requires a non-empty `name` and caps results at 100. `GET /api/member/{id}/details` returns the member with its subscription or `404`.
- `GET /api/subscription` lists all tiers ordered by price, `GET /api/subscription/{id}` returns one. `POST`/`PUT` validate `weekly` 1–7, `duration` 1–520 (weeks) and `price` ≥ 0. `DELETE` returns `409` while members still reference the tier.
- `GET /api/trainer` lists trainers, `GET /api/trainer?name=` filters by name (case-insensitive). `DELETE` returns `409` while the trainer is linked to a subscription in `trainer_subs`.
- `POST /api/member` and `PUT /api/member/{id}` reject an unknown `subsId` with `400 "Pretplata ne postoji."` and a taken e-mail with `409`. `DELETE`/`PUT` on an unknown id return `404`.
- `PUT /api/member/me/subscription/{subsId}` lets any logged-in member set or switch their own plan (member id comes from the token's `NameIdentifier`). Unknown plan → `400`, missing profile → `404`, success → the updated member details payload.
- Controllers return DTOs (`memId`, `name`, `email`, `role`, `subsId`, `subsId/weekly/duration/price`, `trainerId/name/email`). Never return entities — `PasswordHash` must not leave the API.
- Seed data currently in `gymap_db` mirrors `PraksaDB` (same UUIDs and values): 4 tiers (7x/1 week 15.00, 3x/4 weeks 45.00, 3x/12 weeks 120.00, 3x/52 weeks 400.00), 4 trainers (Ana Jurić, Ivan Horvat, Marko Kovač, Petra Babić, `@gym.hr`) with 7 `trainer_subs` links, and 5 members (Dominik Livaković, Ivana Perić, Joško Gvardiol, Luka Modrić, Mateo Kovačić, `@gmail.com`), each with a subscription. All five log in with `12345678`; **`luka@gmail.com` is the only Admin**, the rest are `User`.
- Token lifetime comes from `JwtSettings:ExpirationInMinutes` (default 120); `JwtSettings:Key` must be at least 32 bytes.
- `LoggingMW` appends `audit_log.txt` to the API process CWD for successful non-GET requests; that file is not gitignored.

## Frontend notes

- All API calls go through `apiFetch` in `src/api.js` (relative `api/...` paths via the Vite proxy); it attaches the bearer token, translates the API's Croatian messages into English (see `MESSAGES`) and emits `auth:expired` on 401. `VITE_API_URL` in `frontend/my-app/.env` is unused.
- `AuthProvider` (`src/AuthProvider.jsx`) reads the JWT from `localStorage`, decodes `memberId/email/role/exp` (`decodeToken`), auto-logs out on expiry, and exposes `useAuth()` from `src/auth-context.js`. `main.jsx` wires the provider and imports both stylesheets, so direct loads of `/login` and `/search` are styled.
- Auth and member search are no longer duplicated: `/` (`App.jsx`) is a landing page for guests (hero + plans) and a dashboard for logged-in users (plans, member search, coaches), `/login` is the only auth form (centred card, no side panel), `/search` and the dashboard share `components/MemberSearch.jsx`.
- The guest plan list starts compact — `SubscriptionCard` gets `compact` (name + price only) and the section carries `.plans-preview` to sit lower — and the hero's "See plans" button toggles `detailed` in `Home` (the label flips to "Hide details") to show the `3× a week · 4 weeks` line. `Get started` links to `/login?mode=register`, which opens the Sign up tab (`Login.jsx` reads `mode` from the query string) plus a one-line motivational note. The guest `/search` page is only a one-line card; the header keeps the single login button.
- On the dashboard each plan card has a choose/switch button that calls `PUT /api/member/me/subscription/{subsId}` and refreshes the membership card from the response; buttons only render once the real plan list has loaded (the `FALLBACK_PLANS` ids are not valid GUIDs).
- The UI is deliberately plain: one 960px `.container`, `.card`/`.plan` panels with a 1px border, no shadows or gradients, and two corner radii (8px/12px). Copy stays short (headline + one line). Type is **Manrope** for headings and prices (tight tracking, tabular numbers) and **Inter** for body copy, both imported in `index.css`. `src/App.css` is organised by section with plain class names (`.page`, `.dash-head`, `.plan-grid`, `.coach-list`); keep new styles there rather than adding decorative layers. Plan grids go 1 column → 2 at 560px → 4 at 900px.
- The API's `auth` response has an `ActiveOffer` field that is never populated, and registration takes `name` (not `selectedOffer`). On the dashboard the active plan comes from `GET /api/member/{id}/details`, with `memId` read from the token.
- `GET /api/subscription` and `/api/trainer` require a token, so `src/subscriptions.js` keeps a `FALLBACK_PLANS` list that mirrors the seed for the guest landing page. Keep it in sync with the seed if tiers change.
